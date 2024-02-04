using SkiaSharp;

public class Image8BitHeightMap : IHeightMap
{
    private SKBitmap bitmap;
    private Map2dIterator _iterator;
    public Image8BitHeightMap(SKBitmap bitmap)
    {
        _iterator = new Map2dIterator((bitmap.Width, bitmap.Height));
        this.bitmap = bitmap;
    }
    public IMapIterator<(int,int)> iterator()
    {
        return _iterator;
    }

    public (int x, int y) Bounds()
    {
        return (bitmap.Width, bitmap.Height);
    }

    public short GetHeight(int x, int y)
    {
        return bitmap.GetPixel(x,y).Red;
    }

    public bool inBounds(int x, int y)
    {
       return x >= 0 && y >= 0 && x < Bounds().x && y < Bounds().y;
    }

    public void SetHeight((int x, int y) pos, short z)
    {
        bitmap.SetPixel(pos.x, pos.y, new SKColor((byte)z, (byte)z, (byte)z));
    }
}