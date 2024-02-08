using application.Maps.flowMap;
using cFlowAPI.Maps.riverMap;
using SkiaSharp;

namespace src.Maps.riverMap
{
    public class RiverMap : Map2d
    {
        private readonly bool[][] map;
        private readonly DistanceMap distanceMap;
        private Map2dIterator _iterator;
        private IHeightMap _heightMap;
        private SKBitmap riverOverlay;
        public RiverMap(DistanceMap distanceMap, IHeightMap heightMap)
        {
            this.distanceMap = distanceMap;
            this._heightMap = heightMap;
            map = new bool[distanceMap.Bounds().x][];
            _iterator = new Map2dIterator(distanceMap.Bounds());
            for (int i = 0; i < distanceMap.Bounds().x; i++)
            {
                map[i] = new bool[distanceMap.Bounds().y];
            }
            riverOverlay = new SKBitmap(new SKImageInfo(Bounds().x, Bounds().y, SKColorType.Rgba8888, SKAlphaType.Opaque));

        }

        public SKBitmap ToImage()
        {
            return riverOverlay;
        }

        public void SetAsRiver(int x, int y)
        {
            map[x][y] = true;
            riverOverlay.SetPixel(x, y, new SKColor(0, 0, 255));
        }

        public bool IsRiver(int x, int y)
        {
            return map[x][y];
        }

        public void AddRiverFrom((int x, int y) pos, int branchEveryX = 100)
        {
            Random random = new Random();
            var start = pos;
            var stopped = false;
            var doSplitNow = false;
            while (!stopped)
            {
                if (IsRiver(start.x, start.y))
                    break;
                SetAsRiver(start.x, start.y);

                if (!doSplitNow && branchEveryX != -1)
                {
                    //perform a split every x blocks on average
                    doSplitNow = (random.Next() % branchEveryX) == 0;
                }

                var (stop, next) = AdvanceRiver(start, random, doSplitNow ? 2 : 1);
                if (doSplitNow && next.Count > 1)
                {
                    AddRiverFrom(next[0], branchEveryX);
                    AddRiverFrom(next[1], branchEveryX);
                    break;
                }
                start = next[0];
                stopped = stop;
            }

            //FIXME smart way to escape flooded area an continue river
            if (stopped)
                new FloodTool(_heightMap).FloodArea(start, this);
        }

        /// <summary>
        /// continues path along the flow.
        /// will return startFlow if not possible to continue
        /// </summary>
        /// <param name="startFlow"></param>
        /// <returns></returns>
        private (bool stopped, List<(int x, int y)> next) AdvanceRiver((int x, int y) startFlow, Random random, int branches = 1)
        {
            var candidates = distanceMap.FlowFrom(startFlow);
            if (candidates.Count == 0)
                return (true, [startFlow]);
            var outList = new List<(int x, int y)>();

            //randomly take from candidates list until enough branches are collected or no more candidates remain
            while (outList.Count < branches && candidates.Count != 0)
            {
                var randomIdx = random.Next() % candidates.Count;
                var taken = candidates[randomIdx];
                candidates.RemoveAt(randomIdx);
                outList.Add(taken);
            }
            return (false, outList);
        }



        public (int x, int y) Bounds()
        {
            return distanceMap.Bounds();
        }

        public bool inBounds(int x, int y)
        {
            return distanceMap.inBounds(x, y);
        }

        public IMapIterator<(int x, int y)> iterator()
        {
            return _iterator;
        }
    }
}
