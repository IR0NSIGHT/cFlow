using System.Drawing;
using System.Drawing.Imaging;


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
        var shadedHeightMap = new Bitmap(Bounds().x, Bounds().y, PixelFormat.Format16bppGrayScale);
        var contourMap = new Bitmap(Bounds().x, Bounds().y, PixelFormat.Alpha);
        byte alpha = 127;

        var flowMap = new SimpleFlowMap(Bounds());
        SimpleFlowMap.ApplyNaturalEdgeFlow(this, flowMap);

        foreach (var (x, y) in flowMap.iterator().Points())
        {
            var flow = flowMap.GetFlow((x, y));
            byte val = 127;
            if (flow.Up)
                val += 50;
            if (flow.Left)
                val += 50;


            if (flow.Down)
                val -= 50;
            if (flow.Right)
                val -= 50;

            shadedHeightMap.SetPixel(x, y, Color.FromArgb(255, val, val, val));

            if (!flow.Unknown && GetHeight((x,y)) % 10 == 0) 
                contourMap.SetPixel(x,y,Color.FromArgb(alpha, Color.Black));
        }

        return (shadedHeightMap, contourMap);
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