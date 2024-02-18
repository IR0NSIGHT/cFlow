using System.Drawing;
using cFlowAPI.Maps.Shader;

public class DummyDimension : IHeightMap
{
    private Map2dIterator _iterator;

    public DummyDimension((int x, int y) size, short height)
    {
        heightMap = filledHeightmap(size, height);
        _iterator = new Map2dIterator(size);
    }

    public (Bitmap, Bitmap) ShadedHeightmap()
    {
    //    var sunlightMap = MapShaderApi.SunlightFromHeightmap(heightmap: bitmap);
    //    var contourMap = MapShaderApi.ContourFromHeightmap(heightmap: bitmap);
        return (new Bitmap(30,30), new Bitmap(30, 30));
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
}
