using System.Drawing;

public class ImageApi
{
    public static Bitmap LoadBitmapFromPng(string filePath)
    {
        var bitmap = new Bitmap(File.OpenRead(filePath));
        return bitmap;
    }

    public static void SaveBitmapAsPng(Bitmap bitmap, string outputPath)
    {
        try
        {
            bitmap.Save(outputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving image: {ex.Message}");
        }
    }
}
