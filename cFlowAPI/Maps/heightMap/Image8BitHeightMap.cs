using System.Drawing;
using System.Drawing.Imaging;
using cFlowAPI.Maps.Shader;


public class Image8BitHeightMap : IHeightMap
{
    private Bitmap bitmap;
    private Map2dIterator _iterator;
    public Image8BitHeightMap(Bitmap bitmap)
    {
        _iterator = new Map2dIterator((bitmap.Width, bitmap.Height));
        this.bitmap = bitmap;
    }
    
    public (Bitmap, Bitmap) ShadedHeightmap()
    {
        //TODO create contour map too
        var sunlightMap = MapShaderApi.SunlightFromHeightmap(heightmap: bitmap);
        var contourMap = MapShaderApi.ContourFromHeightmap(heightmap: bitmap);
        return (sunlightMap, contourMap);
    }

    public IMapIterator<(int, int)> iterator()
    {
        return _iterator;
    }

    public (int x, int y) Bounds()
    {
        return (bitmap.Width, bitmap.Height);
    }

    public short GetHeight((int x, int y) pos)
    {
        return bitmap.GetPixel(pos.x, pos.y).R;
    }

    public bool inBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Bounds().x && y < Bounds().y;
    }

    public void SetHeight((int x, int y) pos, short z)
    {
        bitmap.SetPixel(pos.x, pos.y, Color.FromArgb(255,(byte)z, (byte)z, (byte)z));
    }
}