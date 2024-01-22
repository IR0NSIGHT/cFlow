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
        return (bitmap.Height, bitmap.Width);
    }

    public short GetHeight(int x, int y)
    {
        return bitmap.GetPixel(x,y).Red;
    }

    public bool inBounds(int x, int y)
    {
       return x >= 0 && y >= 0 && x < Bounds().x && y < Bounds().y;
    }


    public void SetHeight(Point point)
    {
        bitmap.SetPixel(point.X, point.Y, new SKColor((byte)point.height, (byte)point.height, (byte)point.height));
    }
}