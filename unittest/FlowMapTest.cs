using SkiaSharp;

namespace tests
{
    [TestFixture]
    public class FlowMapTest
    {
        //[Test] //FIXME suspected to lead to issues with test runner for some reason?
        public void TestMapOrientation()
        {
            var dims = (10, 3);
            var fMap = new SimpleFlowMap(dims);

            //mark y = 0 ==> first column 

            fMap.SetFlow(0, 0, new IFlowMap.Flow(false, true, false, false, false));
            Assert.IsTrue(fMap.GetFlow(0, 0).Flow.Up);
            fMap.SetFlow(9, 0, new IFlowMap.Flow(false, true, false, false, false));
            Assert.IsTrue(fMap.GetFlow(9, 0).Flow.Up);

            //picture origin is top left = 0,0
            //width = x
            //height = y


            fMap.SetFlow(9, 2, new IFlowMap.Flow(false, false, true, false, false));
            Assert.IsTrue(fMap.GetFlow(9, 2).Flow.Down);

            var bitmap = SimpleFlowMap.ToColorImage(fMap);
            //ImageApi.SaveBitmapAsPng(bitmap, "C:\\Users\\Max1M\\OneDrive\\Bilder\\debug_flowmap.png");

        }

        [Test]
        public void NaturalFlow() {
            //test if depressresions in the terrain will cause neighboursing blocks to be marked as natural edge flow

            var heightmap = new DummyDimension((10, 3), 17);
            heightmap.SetHeight(new Point(0, 0, 5));

            var flowMap = new SimpleFlowMap(heightmap.Bounds());
            SimpleFlowMap.ApplyNaturalEdgeFlow(heightmap, flowMap);
            foreach ( var point in flowMap.GetPoints())
            {
                var (x,y) = (point.X, point.Y);
                if ((x,y) == (0,1) )
                {
                    Assert.AreEqual(new IFlowMap.Flow(false, false, true, false, false), point.Flow);
                } else if ((x, y) == (1, 0))
                {
                    Assert.AreEqual(new IFlowMap.Flow(false, false, false, true, false), point.Flow);
                } else
                {
                    Assert.AreEqual(new IFlowMap.Flow(true, false, false, false, false), point.Flow);
                }
            }

            var bitmap = SimpleFlowMap.ToImage(flowMap);
            Assert.IsNotNull(bitmap);
            var color1 = bitmap.GetPixel(0, 1);
            var color2 = bitmap.GetPixel(1,0);
            Console.WriteLine(color2 + " " + color1);
        }

        [Test]
        public void fromImageToHeightmapToFlowMapToImage()
        {
            //integration test 
            //image => heightmap => flowmap => image

            var (width, height) = (10, 3);
            var bitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
            //FIXME rest of bitmap is zero => zero height
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bitmap.SetPixel(x, y, new SKColor(102,102,102));
                }
            }

            //mark edges top left, top right, bottom right in brighter getting grays
            bitmap.SetPixel(0, 0, new SKColor(27, 27, 27));
            bitmap.SetPixel(9, 0, new SKColor(47, 47, 47));
            bitmap.SetPixel(9, 2, new SKColor(77, 77, 77));

            var heightmap = new Image8BitHeightMap(bitmap);
            Assert.That(heightmap.GetHeight(0, 0), Is.EqualTo(27));
            Assert.That(heightmap.GetHeight(9, 0), Is.EqualTo(47));
            Assert.That(heightmap.GetHeight(9, 2), Is.EqualTo(77));

            var flowMap = new SimpleFlowMap(heightmap.Bounds()).FromHeightMap(heightmap);
            Assert.That(heightmap.Bounds(), Is.EqualTo(flowMap.getDimensions()));

            SimpleFlowMap.ApplyNaturalEdgeFlow(heightmap, flowMap);
        //    EntryClass.SaveToFile(FlowMapPrinter.FlowMapToString(flowMap, heightmap), false);

            var unknownFlow = new IFlowMap.Flow(true, false, false, false, false);
            Assert.That(flowMap.GetFlow(0, 0).Flow, Is.EqualTo(unknownFlow));
            Assert.That(flowMap.GetFlow(9, 0).Flow, Is.EqualTo(unknownFlow));
            Assert.That(flowMap.GetFlow(9, 2).Flow, Is.EqualTo(unknownFlow));


            var flowImageGray8 = SimpleFlowMap.ToImage(flowMap);
            Assert.That(flowImageGray8.Height, Is.EqualTo(height));
            Assert.That(flowImageGray8.Width, Is.EqualTo(width));

            var c1 = (uint)flowImageGray8.GetPixel(0, 0);
            var c2 = (uint)flowImageGray8.GetPixel(9, 0);
            var c3 = (uint)flowImageGray8.GetPixel(9, 2);
            Assert.That(c1, Is.EqualTo(0xFFFFFFFF));
            Assert.That(c2, Is.EqualTo(0xFFFFFFFF));
            Assert.That(c3, Is.EqualTo(0xFFFFFFFF));

            Assert.That((uint)flowImageGray8.GetPixel(0, 1), Is.Not.EqualTo(0xFFFFFFFF));
            Assert.That((uint)flowImageGray8.GetPixel(1, 0), Is.Not.EqualTo(0xFFFFFFFF));

            Assert.That((uint)flowImageGray8.GetPixel(9, 1), Is.Not.EqualTo(0xFFFFFFFF));
            Assert.That((uint)flowImageGray8.GetPixel(8, 0), Is.Not.EqualTo(0xFFFFFFFF));

            Assert.That((uint)flowImageGray8.GetPixel(9, 1), Is.Not.EqualTo(0xFFFFFFFF));
            Assert.That((uint)flowImageGray8.GetPixel(8, 2), Is.Not.EqualTo(0xFFFFFFFF));


        //    ImageApi.SaveBitmapAsPng(flowImageGray8, "./debug_flowtest");
        }
    }
}
