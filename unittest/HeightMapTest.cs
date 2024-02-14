using System.Drawing;
using System.Drawing.Imaging;

namespace TestProject1
{
    [TestFixture]
    public class HeightMapTest
    {
        [Test]
        public void InitHeightMap()
        {
            var heightmap = new DummyDimension((8, 12), 17);
            Assert.AreEqual((8, 12), heightmap.Bounds());

            foreach (var point in heightmap.iterator().Points())
            {
                Assert.AreEqual(17, heightmap.GetHeight(point));
            }


        }
        [Test]
        public void ChangeHeight()
        {
            var heightmap = new DummyDimension((8, 12), 17);
            heightmap.SetHeight((7, 5), 3);
            Assert.AreEqual(3, heightmap.GetHeight((7, 5)));

            //only changed height of that single point
            foreach(var point in heightmap.iterator().Points())
            {
                if (!(point.x == 7 && point.y == 5))
                    Assert.AreEqual(17, heightmap.GetHeight(point));
            }
        }

        [Test]
        public void IterateLines()
        {
            //iterates lines = increase y
            //start at zero
            //iterates row = increase x
            var heightmap = new DummyDimension((8, 12), 17);
            int row = 0;
            foreach(var iterateLine in heightmap.iterator().PointsByRow())
            {
                int column = 0;
                foreach (var point in iterateLine)
                {
                    Assert.AreEqual(row, point.y);
                    Assert.AreEqual(column, point.x);
                    column++; 
                }
                row ++;
            }
        }

        [Test]
        public void FromImage()
        {
            var (width, height) = (10, 3);
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //mark edges top left, top right, bottom right in brighter getting grays
            bitmap.SetPixel(0, 0, Color.FromArgb(255,27,27,27));
            bitmap.SetPixel(9, 0, Color.FromArgb(255, 47, 47, 47));
            bitmap.SetPixel(9, 2, Color.FromArgb(255, 77, 77, 77));

            var heightmap = new Image8BitHeightMap(bitmap);
            Assert.AreEqual(27, heightmap.GetHeight((0, 0)));
            Assert.AreEqual(47, heightmap.GetHeight((9, 0)));
            Assert.AreEqual(77, heightmap.GetHeight((9, 2)));
        }
    }
}