using System.Drawing;
using cFlowApi.Heightmap;

namespace unittest;

[TestFixture]
public class LargeImageTest
{
    [Test]
    public void LargeImgTest()
    {
        string imagePath = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\30x30_testmap.png";
        DummyDimension.ImportFromFile(imagePath);
    }

    [Test]
    public void correctPixelData()
    {
        Bitmap bmp = new Bitmap(10, 20);
        for (int i = 0; i < bmp.Width; i++)
        {
            bmp.SetPixel(i,i,Color.FromArgb(255,19,15,27));
        }

        var data = DummyDimension.pixel16bitHeighmapArray(bmp);
        for (int i = 0; i < bmp.Width; i++)
        {
            Assert.That(data[i][i], Is.EqualTo(15<<8 | 27));
        }
    }

    [Test]
    public void shiftedPixelData()
    {
        Bitmap bmp = new Bitmap(10, 20);
        for (int i = 0; i < bmp.Width; i++)
        {
            bmp.SetPixel(i, i, Color.FromArgb(255, 19, 15, 27));
        }

        int widthOffset = 20;
        int heightOffset = 30;
        var data = DummyDimension.arrayOfSize(100, 100);

        DummyDimension.pixel16bitHeighmapArray(bmp, data, widthOffset, heightOffset);
        for (int i = 0; i < bmp.Width; i++)
        {
            Assert.That(data[i+heightOffset][i+widthOffset], Is.EqualTo(15 << 8 | 27));
        }
    }
}