namespace application.Maps
{
    internal class BooleanMap : Map2d
    {
        private IMapIterator<(int x, int y)> _iterator;
        private (int x, int y) _bounds;
        private bool[][] seenMap;
        public BooleanMap((int x, int y) bounds)
        {
            this._bounds = bounds;
            this._iterator = new Map2dIterator(bounds);
            seenMap = new bool[bounds.x][];
            for (int x = 0; x < bounds.x; x++)
            {
                seenMap[x] = new bool[bounds.y];
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
            seenMap[x][y] = true;
        }
    }
}
