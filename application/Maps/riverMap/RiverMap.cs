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
            map = new bool[flowMap.getDimensions().x][];
            _iterator = new Map2dIterator(flowMap.getDimensions());
            for (int i = 0; i < flowMap.getDimensions().x; i++)
            {
                map[i] = new bool[flowMap.getDimensions().y];
            }
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
            var start = flowMap.GetFlow(x, y);
            var stopped = false;
            while (!stopped)
            {
                var (stop, next) = AdvanceRiver(start, random);
                SetAsRiver(next.X, next.Y);
                stopped = stop;
            }
        }

        /// <summary>
        /// continues path along the flow.
        /// will return startFlow if not possible to continue
        /// </summary>
        /// <param name="startFlow"></param>
        /// <returns></returns>
        private (bool stopped, IFlowMap.PointFlow next) AdvanceRiver(IFlowMap.PointFlow startFlow, Random random)
        {
            var candidates = flowMap.FollowFlow(startFlow);
            if (candidates.Count == 0)
                return (true, startFlow);
            return (false, candidates[random.Next() % candidates.Count]);
        }



        public (int x, int y) Bounds()
        {
            return flowMap.getDimensions();
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
