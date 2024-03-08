using System.Drawing;
using System.Drawing.Imaging;
using cFlowApi.Heightmap;

namespace TestProject1
{
    [TestFixture]
    public class HeightMapTest
    {
        [Test]
        public void InitHeightMap()
        {
            var heightmap = new HeightMap((8, 12), 17);
            Assert.AreEqual((8, 12), heightmap.Bounds());

            foreach (var point in heightmap.iterator().Points())
            {
                Assert.AreEqual(17, heightmap.GetHeight(point));
            }


        }
        [Test]
        public void ChangeHeight()
        {
            var heightmap = new HeightMap((8, 12), 17);
            heightmap.SetHeight((7, 5), 3);
            Assert.AreEqual(3, heightmap.GetHeight((7, 5)));

            //only changed height of that single point
            foreach (var point in heightmap.iterator().Points())
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
            var heightmap = new HeightMap((8, 12), 17);
            int row = 0;
            foreach (var iterateLine in heightmap.iterator().PointsByRow())
            {
                int column = 0;
                foreach (var point in iterateLine)
                {
                    Assert.AreEqual(row, point.y);
                    Assert.AreEqual(column, point.x);
                    column++;
                }
                row++;
            }
        }

        [Test]
        public void FromImage()
        {
            var (width, height) = (10, 3);
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //mark edges top left, top right, bottom right in brighter getting grays
            bitmap.SetPixel(0, 0, Color.FromArgb(255, 27, 27, 27));
            bitmap.SetPixel(9, 0, Color.FromArgb(255, 47, 47, 47));
            bitmap.SetPixel(9, 2, Color.FromArgb(255, 77, 77, 77));

            Assert.Fail("from image not implemented.");
        }

        [Test]
        public void Merge2MapsSameSize()
        {
            //same size
            var merged = HeightMap.Merge(new HeightMap((10, 20), 17), new HeightMap((10, 20), 13), (0, 0));
            foreach (var point in merged.iterator().Points())
            {
                Assert.That(merged.GetHeight(point), Is.EqualTo(17 + 13));
            }
        }

        [Test]
        public void Merge2MapsDifferentSizeNoOffset()
        {
            var aMap = new HeightMap((10, 20), 17);
            var bMap = new HeightMap((100, 200), 13);
            var merged = HeightMap.Merge(aMap, bMap, (0, 0));

            Assert.That(merged.Bounds(), Is.EqualTo(bMap.Bounds()));
            foreach (var point in merged.iterator().Points())
            {
                if (aMap.inBounds(point) && bMap.inBounds(point))
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(17 + 13));

                }
                else
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(13));
                }
            }

            Assert.That(merged.ToGPUdata(), Is.EqualTo(HeightMap.Merge(aMap, bMap, (0, 0)).ToGPUdata()));
        }

        [Test]
        public void Merge2MapsDifferentSizeWithOffsetNoResize()
        {
            var staticMap = new HeightMap((5, 10), 13);
            var shiftedMap = new HeightMap((2, 4), 17);
            var shift = (2, 3);
            var merged = HeightMap.Merge(staticMap, shiftedMap, shift);

            Assert.That(merged.Bounds(), Is.EqualTo(staticMap.Bounds()));

            foreach (var point in merged.iterator().Points())
            {
                var unshiftedPoint = (point.x - shift.Item1, point.y - shift.Item2);

                if (shiftedMap.inBounds(unshiftedPoint))
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(17 + 13), $"point {point} (unshifted:{unshiftedPoint}) is off");
                }
                else
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(staticMap.GetHeight(point)));
                }
            }

            var data = new ushort[,]
            {
                { 13 ,13 ,13 ,13 ,13 },
                { 13 ,13 ,13 ,13 ,13 },
                { 13 ,13 ,13 ,13 ,13 },
                { 13 ,13 ,30 ,30 ,13 },
                { 13 ,13 ,30 ,30 ,13 },
                { 13 ,13 ,30 ,30 ,13 },
                { 13 ,13 ,30 ,30 ,13 },
                { 13 ,13 ,13 ,13 ,13 },
                { 13 ,13 ,13 ,13 ,13 },
                { 13 ,13 ,13 ,13 ,13 },

            };

            Assert.That(merged.ToGPUdata(), Is.EqualTo(data));
        }

        [Test]
        public void Merge2MapsDifferentShapes()
        {
            var staticMap = new HeightMap((1, 10), 13);
            var shiftedMap = new HeightMap((10, 1), 17);
            var shift = (0,0);
            var merged = HeightMap.Merge(staticMap, shiftedMap, shift);

            Assert.That(merged.Bounds(), Is.EqualTo((10,10)));

            foreach (var point in merged.iterator().Points())
            {
                if (shiftedMap.inBounds(point) && staticMap.inBounds(point))
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(17 + 13));
                } else if (shiftedMap.inBounds(point)) {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(17));
                } else if(staticMap.inBounds(point))
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(13));
                } else
                {
                    Assert.That(merged.GetHeight(point), Is.EqualTo(0));
                }
            }
        }
    }
}