using System.Drawing;
using static FlowTranslation;
namespace namespace_01
{
    [TestFixture]
    public class FlowColorTest
    {
        [Test]
        public void FlowToColorGray8()
        {
            Assert.That(FlowToGray8(new IFlowMap.Flow(true, false, false, false, false)), Is.EqualTo(255));
            //0000
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, false, false, false)), Is.EqualTo(0));
            //0001
            Assert.AreEqual(1 * 7, FlowToGray8(new IFlowMap.Flow(false, false, false, false, true)));
            //0010
            Assert.AreEqual(2 * 7, FlowToGray8(new IFlowMap.Flow(false, false, false, true, false)));
            //0011
            Assert.AreEqual(3 * 7, FlowToGray8(new IFlowMap.Flow(false, false, false, true, true)));
            //0100
            Assert.AreEqual(4 * 7, FlowToGray8(new IFlowMap.Flow(false, false, true, false, false)));
            //0101
            Assert.AreEqual(5 * 7, FlowToGray8(new IFlowMap.Flow(false, false, true, false, true)));
            //0110
            Assert.AreEqual(6 * 7, FlowToGray8(new IFlowMap.Flow(false, false, true, true, false)));
            //0111
            Assert.AreEqual(7 * 7, FlowToGray8(new IFlowMap.Flow(false, false, true, true, true)));

            //1000
            Assert.AreEqual(8 * 7, FlowToGray8(new IFlowMap.Flow(false, true, false, false, false)));
            //1001
            Assert.AreEqual(9 * 7, FlowToGray8(new IFlowMap.Flow(false, true, false, false, true)));
            //1010
            Assert.AreEqual(10 * 7, FlowToGray8(new IFlowMap.Flow(false, true, false, true, false)));
            //1011
            Assert.AreEqual(11 * 7, FlowToGray8(new IFlowMap.Flow(false, true, false, true, true)));
            //1100
            Assert.AreEqual(12 * 7, FlowToGray8(new IFlowMap.Flow(false, true, true, false, false)));
            //1101
            Assert.AreEqual(13 * 7, FlowToGray8(new IFlowMap.Flow(false, true, true, false, true)));
            //1110
            Assert.AreEqual(14 * 7, FlowToGray8(new IFlowMap.Flow(false, true, true, true, false)));
            //1111
            Assert.AreEqual(15 * 7, FlowToGray8(new IFlowMap.Flow(false, true, true, true, true)));
        }

        [Test]
        public void ColorOnFlowMap()
        {
            var flowMap = new SimpleFlowMap((16, 7)).FromHeightMap(new DummyDimension((16, 7), 13));
            IFlowMap.Flow[] allFlows = {
                new IFlowMap.Flow(false, false, false, false, false),
                new IFlowMap.Flow(false, false, false, false, true),
                new IFlowMap.Flow(false, false, false, true, false),
                new IFlowMap.Flow(false, false, false, true, true),
                new IFlowMap.Flow(false, false, true, false, false),
                new IFlowMap.Flow(false, false, true, false, true),
                new IFlowMap.Flow(false, false, true, true, false),
                new IFlowMap.Flow(false, false, true, true, true),
                new IFlowMap.Flow(false, true, false, false, false),
                new IFlowMap.Flow(false, true, false, false, true),
                new IFlowMap.Flow(false, true, false, true, false),
                new IFlowMap.Flow(false, true, false, true, true),
                new IFlowMap.Flow(false, true, true, false, false),
                new IFlowMap.Flow(false, true, true, false, true),
                new IFlowMap.Flow(false, true, true, true, false),
                new IFlowMap.Flow(false, true, true, true, true)
            };
            for (int i = 0; i < 15; i++)
            {
                flowMap.SetFlow(i, 3, allFlows[i]);
                var flowOnMap = flowMap.GetFlow(i, 3).Flow;
                Assert.That(i * 7, Is.EqualTo(FlowToGray8(flowOnMap)));
            }

            //check that image get correct colors
            var image = SimpleFlowMap.ToImage(flowMap);
            Assert.IsNotNull(image);

            //first 15 bytes in a row = first 15 pixels, y=3 == row=4 == offset 3*width == 4*16 => first 15 colors 0..15 
            for (int i = 0; i < 15; i++)
            {
                var color = image.Bytes[3 * 16 + i];
                Assert.That(i * 7, Is.EqualTo(color));
            }


        }
    }
}
