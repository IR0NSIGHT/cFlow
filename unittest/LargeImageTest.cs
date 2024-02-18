using System.Drawing.Imaging;
using System.Drawing;
using ComputeSharp;
using TerraFX.Interop.Windows;

namespace unittest;

[TestFixture]
public class LargeImageTest
{

    //TODO use this maybe? https://github.com/drewnoakes/metadata-extractor-dotnet/tree/main
    [Test]
    public void LargeImgTest()
    {
        string imagePath = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\30x30_testmap.png";
        var tileSize = 1028;
        using (var fileStream = new FileStream(imagePath, FileMode.Open))
        {
            using (var image = Image.FromStream(fileStream, false, false))
            {
                int width = image.Width;
                int height = image.Height;
                var bitmap =  new Bitmap(1028, 1028);
                Rectangle sourceRect = new Rectangle(0, 0, Math.Min(width, bitmap.Width), Math.Min(height, bitmap.Height));
                // Create a graphics object from the bitmap
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Use Graphics.DrawImage() to paint the original PNG image onto the bitmap
                    graphics.DrawImage(image, sourceRect, sourceRect, GraphicsUnit.Pixel);

                }
                bitmap.Save("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\tinyboi.png");
            }
        }
    }
}