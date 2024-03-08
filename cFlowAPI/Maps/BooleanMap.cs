using System.Diagnostics;
using System.Drawing;
using cFlowApi.Heightmap;

namespace application.Maps
{
    public class BooleanMap : Map2d
    {
        private IMapIterator<(int x, int y)> _iterator;
        private (int x, int y) _bounds;
        private bool[][] seenMap;
        private int marked = 0;
        private (int x, int y) lowerBoundsMarked = (int.MaxValue, int.MaxValue);
        private (int x, int y) upperBoundsMarked = (0, 0);
        public BooleanMap((int x, int y) bounds)
        {
            this._bounds = bounds;
            this._iterator = new Map2dIterator(bounds);
            upperBoundsMarked = bounds;
            seenMap = HeightMap.arrayOfSize<bool>(bounds.x, bounds.y);
        }

        /// <summary>
        /// 1 = true, 0 = false
        /// </summary>
        /// <param name="data"></param>
        public void FromGpuData(int[,] data)
        {
            for (int y = 0; y < Bounds().y; y++)
            {
                for (int x = 0; x < Bounds().x; x++)
                {
                    seenMap[y][x] = data[y, x] == 1;
                }
            }
        }

        public Bitmap ToImage(int x = 0, int y = 0, int width = -1, int height = -1)
        {
            if (width == -1) width = Bounds().x;
            if (height == -1) height = Bounds().y;

            Debug.Assert(width <= HeightMap.chunkSize);
            Debug.Assert(height <= HeightMap.chunkSize);

            Debug.Assert(x >= 0 && y >= 0);
            Debug.Assert(x + width <= Bounds().x && y + height <= Bounds().y, $"out of bounds with inputDistanceMap: {Bounds()}");
            var bitmap = new Bitmap(width, height);
            foreach (var point in _iterator.Points())
            {
                if (isMarked(point.x, point.y))
                    bitmap.SetPixel(point.x - x, point.y - y, Color.Red);
            }
            return bitmap;
        }

        /// <summary>
        /// clears all values from the map
        /// </summary>
        public void Clear()
        {
            foreach (var subArr in seenMap)
            {
                Array.Clear(subArr);
            }
        }

        /// <summary>
        ///  1 = true, 0 = false
        /// </summary>
        /// <returns></returns>
        public int[,] ToGpuData()
        {
            var data = this.seenMap;
            int[,] result = new int[Bounds().y, Bounds().x];
            for (int y = 0; y < Bounds().y; y++)
            {
                for (int x = 0; x < Bounds().x; x++)
                {
                    result[y, x] = data[y][x] ? 1 : 0;
                }
            }

            return result;
        }

        public (int x, int y) Bounds()
        {
            return _bounds;
        }

        public bool inBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Bounds().x && y < Bounds().y;
        }

        public IMapIterator<(int x, int y)> iterator()
        {
            return _iterator;
        }

        public bool isMarked(int x, int y)
        {
            return seenMap[y][x];
        }

        public void setMarked(int x, int y)
        {
            if (isMarked(x, y))
            {
                return;
            }
            marked++;
            seenMap[y][x] = true;

            if (x < lowerBoundsMarked.x)
                lowerBoundsMarked.x = x;
            if (x > upperBoundsMarked.x)
                upperBoundsMarked.x = x;

            if (y < lowerBoundsMarked.y)
                lowerBoundsMarked.y = y;
            if (y > upperBoundsMarked.y)
                upperBoundsMarked.y = y;
        }

        public int getMarkedAmount()
        {
            return marked;
        }

        public IEnumerable<(int x, int y)> IterateMarked()
        {
            for (var x = lowerBoundsMarked.x; x < upperBoundsMarked.x; x++)
            {
                for (var y = lowerBoundsMarked.y; y < upperBoundsMarked.y; y++)
                {
                    if (isMarked(x, y))
                        yield return (x, y);
                }
            }
        }
    }
}
