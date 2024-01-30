﻿using SkiaSharp;

namespace src.Maps.riverMap
{
    class RiverMap : Map2d
    {
        private bool[][] map;
        private IFlowMap flowMap;
        private Map2dIterator _iterator;
        public RiverMap(IFlowMap flowMap)
        {
            this.flowMap = flowMap;
            map = new bool[flowMap.Bounds().x][];
            _iterator = new Map2dIterator(flowMap.Bounds());
            for (int i = 0; i < flowMap.Bounds().x; i++)
            {
                map[i] = new bool[flowMap.Bounds().y];
            }
        }

        public SKBitmap ToImage()
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(Bounds().x, Bounds().y, SKColorType.Rgba8888, SKAlphaType.Opaque));
            foreach(var point in iterator().Points())
            {
                bitmap.SetPixel(point.x, point.y, IsRiver(point.x, point.y) ? new SKColor(0,0,255) : new SKColor(0,0,0));
            }
            return bitmap;
        }

        public void SetAsRiver(int x, int y)
        {
            map[x][y] = true;
        }

        public bool IsRiver(int x, int y)
        {
            return map[x][y];
        }

        public void AddRiverFrom(int x, int y)
        {
            SetAsRiver(x, y);
            Random random = new Random();
            var start = (x, y);
            var stopped = false;
            while (!stopped)
            {
                var (stop, next) = AdvanceRiver(start, random);
                start = next;
                SetAsRiver(next.x, next.y);
                stopped = stop;
            }
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
