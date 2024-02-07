using cFlowAPI.Maps.riverMap;
using SkiaSharp;

namespace src.Maps.riverMap
{
    public class RiverMap : Map2d
    {
        private readonly bool[][] map;
        private readonly IFlowMap flowMap;
        private Map2dIterator _iterator;
        private IHeightMap _heightMap;
        private SKBitmap riverOverlay;
        public RiverMap(IFlowMap flowMap, IHeightMap heightMap)
        {
            this.flowMap = flowMap;
            this._heightMap = heightMap;
            map = new bool[flowMap.Bounds().x][];
            _iterator = new Map2dIterator(flowMap.Bounds());
            for (int i = 0; i < flowMap.Bounds().x; i++)
            {
                map[i] = new bool[flowMap.Bounds().y];
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
            riverOverlay.SetPixel(x,y, new SKColor(0, 0, 255));
        }

        public bool IsRiver(int x, int y)
        {
            return map[x][y];
        }

        public void AddRiverFrom((int x, int y) pos)
        {
            Random random = new Random();
            var start = pos;
            var stopped = false;
            while (!stopped)
            {
                if (IsRiver(start.x, start.y))
                    break;
                SetAsRiver(start.x, start.y);

                var (stop, next) = AdvanceRiver(start, random);
                start = next;
                stopped = stop;
            }
            //FIXME smart way to escape flooded area an continue river
            new FloodTool(_heightMap).FloodArea(start, this);
        }

        /// <summary>
        /// continues path along the flow.
        /// will return startFlow if not possible to continue
        /// </summary>
        /// <param name="startFlow"></param>
        /// <returns></returns>
        private (bool stopped, (int x, int y) next) AdvanceRiver((int x, int y) startFlow, Random random)
        {
            var candidates = flowMap.FollowFlow(startFlow);
            if (candidates.Count == 0)
                return (true, startFlow);
            return (false, candidates[random.Next() % candidates.Count]);
        }



        public (int x, int y) Bounds()
        {
            return flowMap.Bounds();
        }

        public bool inBounds(int x, int y)
        {
            return flowMap.inBounds(x, y);
        }

        public IMapIterator<(int x, int y)> iterator()
        {
            return _iterator;
        }
    }
}
