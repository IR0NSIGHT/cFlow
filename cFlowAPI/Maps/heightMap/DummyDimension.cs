public class DummyDimension : IHeightMap
{
    private Map2dIterator _iterator;

    public DummyDimension((int x, int y) size, short height)
    {
        heightMap = filledHeightmap(size, height);
        _iterator = new Map2dIterator(size);
    }

    private static short[][] filledHeightmap((int x, int y) size, short height)
    {
        short[][] heightMap = new short[size.x][];
        for (int xIterator = 0; xIterator < size.x; xIterator++)
        {
            heightMap[xIterator] = new short[size.y];
            Array.Fill(heightMap[xIterator], height);
        }

        return heightMap;
    }

    private short[][] heightMap = { [0, 0, 0], [0, 2, 3], [0, 1, 1] };
    public (int x, int y) Bounds()
    {
        int x = heightMap.Length;
        int y = heightMap[0].Length;
        return (x, y);
    }

    public short GetHeight((int x, int y) pos)
    {
        return heightMap[pos.x][pos.y];
    }

    public void SetHeight((int x, int y) pos, short z)
    {
        heightMap[pos.x][pos.y] = z;
    }

    public bool inBounds(int x, int y) =>
        x >= 0 && x < Bounds().x && y >= 0 && y < Bounds().y;

    public IMapIterator<(int x, int y)> iterator()
    {
        return _iterator;
    }

    public static bool hasLowerNeighbours((int X, int Y) p, IHeightMap heightMap)
    {
        short height = heightMap.GetHeight(p);
        bool safeBounds = SimpleFlowMap.insideReducedBounds((p.X, p.Y), heightMap.Bounds());
        return (
            SimpleFlowMap.isLowerThan(Point.Left(p), heightMap, height, safeBounds) ||
            SimpleFlowMap.isLowerThan(Point.Right(p), heightMap, height, safeBounds) ||
            SimpleFlowMap.isLowerThan(Point.Up(p), heightMap, height, safeBounds) ||
            SimpleFlowMap.isLowerThan(Point.Down(p), heightMap, height, safeBounds));

    }
}
