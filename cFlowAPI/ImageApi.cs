using SkiaSharp;
using System.Threading;

public class ImageApi
{
    public static SKBitmap LoadBitmapFromPng(string filePath)
    {
        var bitmap = SKBitmap.Decode(File.OpenRead(filePath));
        return bitmap;
    }

    public static SKBitmap JoinImages(SKBitmap img1, SKBitmap img2)
    {
        var outImg = new SKBitmap(new SKImageInfo(Math.Max(img1.Width, img2.Width), img1.Height + img2.Height, SKColorType.Rgba8888, SKAlphaType.Opaque));
        for (int x = 0; x < img1.Width; x++)
        {
            for (int y = 0; y < img1.Height; y++)
            {
                outImg.SetPixel(x, y, img1.GetPixel(x, y));
            }
        }
        for (int x = 0; x < img2.Width; x++)
        {
            for (int y = 0; y < img2.Height; y++)
            {
                outImg.SetPixel(x, y+img1.Height, img2.GetPixel(x, y));
            }
        }
        return outImg;
    }

    public static void SaveBitmapAsPng(SKBitmap bitmap, string outputPath)
    {
        try
        {
            var image = SKImage.FromBitmap(bitmap);
            var data = image.Encode(SKEncodedImageFormat.Png, 100); // null
            using (var stream = File.OpenWrite(outputPath))
            {
                image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving image: {ex.Message}");
        }
    }
}
