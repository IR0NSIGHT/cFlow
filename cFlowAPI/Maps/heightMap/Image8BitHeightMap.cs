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

    public SKBitmap ContourLinesOverlay()
    {
        var contourMap = new SKBitmap(Bounds().x, Bounds().y, SKColorType.Alpha8, SKAlphaType.Premul);
        foreach (var (x, y) in iterator().Points())
        {
            if (GetHeight((x, y)) % 10 == 0)
                contourMap.SetPixel(x, y, new SKColor(255, 255, 255, 255));
        }

        for (var x = 0; x < bitmap.Width; x++)
        {
            contourMap.SetPixel(x, 0, new SKColor(255, 255, 255, 255));
            contourMap.SetPixel(x, bitmap.Height - 1, new SKColor(255, 255, 255, 255));
        }
        for (var y = 0; y < bitmap.Height; y++)
        {
            contourMap.SetPixel(0, y, new SKColor(255, 255, 255, 255));
            contourMap.SetPixel(bitmap.Width - 1, y, new SKColor(255, 255, 255, 255));
        }
        return contourMap;
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
        return bitmap.GetPixel(pos.x, pos.y).Red;
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