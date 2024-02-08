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
            seenMap = new bool[bounds.x][];
            for (int x = 0; x < bounds.x; x++)
            {
                seenMap[x] = new bool[bounds.y];
            }
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
            return seenMap[x][y];
        }

        public void setMarked(int x, int y)
        {
            if (isMarked(x, y))
            {
                return;
            }
            marked++;
            seenMap[x][y] = true;

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
