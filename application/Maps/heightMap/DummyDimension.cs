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

    public short GetHeight(int x, int y)
    {
        return heightMap[x][y];
    }

    public void SetHeight(Point point)
    {
        heightMap[point.X][point.Y] = point.height;

    }

    public bool inBounds(int x, int y) =>
        x >= 0 && x < Bounds().x && y >= 0 && y < Bounds().y;

    public IMapIterator<(int x, int y)> iterator()
    {
        return _iterator;
    }
}
