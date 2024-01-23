using Microsoft.VisualStudio.TestPlatform.TestHost;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    [TestClass]
    public class FlowMapTest
    {
        [TestMethod]
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
            ImageApi.SaveBitmapAsPng(bitmap, "C:\\Users\\Max1M\\OneDrive\\Bilder\\debug_flowmap.png");

        }

        [TestMethod]
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

        [TestMethod]
        public void fromImageToHeightmapToFlowMapToImage()
        {
            //integration test 
            //image => heightmap => flowmap => image

            var (width, height) = (10, 3);
            var bitmap = new SKBitmap(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
            //mark edges top left, top right, bottom right in brighter getting grays
            bitmap.SetPixel(0, 0, new SKColor(27, 27, 27));
            bitmap.SetPixel(9, 0, new SKColor(47, 47, 47));
            bitmap.SetPixel(9, 2, new SKColor(77, 77, 77));

            var heightmap = new Image8BitHeightMap(bitmap);
            Assert.AreEqual(27, heightmap.GetHeight(0, 0));
            Assert.AreEqual(47, heightmap.GetHeight(9, 0));
            Assert.AreEqual(77, heightmap.GetHeight(9, 2));

            var flowMap = new SimpleFlowMap(heightmap.Bounds()).FromHeightMap(heightmap);
            Assert.AreEqual(flowMap.getDimensions(), heightmap.Bounds());

            SimpleFlowMap.ApplyNaturalEdgeFlow(heightmap, flowMap);
            EntryClass.SaveToFile(FlowMapPrinter.FlowMapToString(flowMap, heightmap), false);

            var unknownFlow = new IFlowMap.Flow(true, false, false, false, false);
            Assert.AreEqual(unknownFlow, flowMap.GetFlow(0, 0).Flow);
            Assert.AreEqual(unknownFlow, flowMap.GetFlow(9, 0).Flow);
            Assert.AreEqual(unknownFlow, flowMap.GetFlow(9, 2).Flow);


            var flowImageGray8 = SimpleFlowMap.ToImage(flowMap);
            Assert.AreEqual(height, flowImageGray8.Height);
            Assert.AreEqual(width, flowImageGray8.Width);

            var c1 = flowImageGray8.GetPixel(0, 0);
            var c2 = flowImageGray8.GetPixel(9, 0);
            var c3 = flowImageGray8.GetPixel(9, 2);
            Console.WriteLine(c1 + " " + c2);
            ImageApi.SaveBitmapAsPng(flowImageGray8, "./debug_flowtest");
        }
    }
}
