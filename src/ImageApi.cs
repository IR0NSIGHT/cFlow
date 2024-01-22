using SkiaSharp;
using System.Threading;

class ImageApi
{
    public static SKBitmap LoadBitmapFromPng(string filePath)
    {
        var bitmap = SKBitmap.Decode(File.OpenRead(filePath));
        return bitmap;
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
