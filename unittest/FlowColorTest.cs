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
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, false, false, true)), Is.EqualTo(1 * 7));
            //0010
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, false, true, false)), Is.EqualTo(2 * 7));
            //0011
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, false, true, true)), Is.EqualTo(3 * 7));
            //0100
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, true, false, false)), Is.EqualTo(4 * 7));
            //0101
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, true, false, true)), Is.EqualTo(5 * 7));
            //0110
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, true, true, false)), Is.EqualTo(6 * 7));
            //0111
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, false, true, true, true)), Is.EqualTo(7 * 7));

            //1000
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, false, false, false)), Is.EqualTo(8 * 7));
            //1001
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, false, false, true)), Is.EqualTo(9 * 7));
            //1010
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, false, true, false)), Is.EqualTo(10 * 7));
            //1011
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, false, true, true)), Is.EqualTo(11 * 7));
            //1100
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, true, false, false)), Is.EqualTo(12 * 7));
            //1101
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, true, false, true)), Is.EqualTo(13 * 7));
            //1110
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, true, true, false)), Is.EqualTo(14 * 7));
            //1111
            Assert.That(FlowToGray8(new IFlowMap.Flow(false, true, true, true, true)), Is.EqualTo(15 * 7));
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
                flowMap.SetFlow((i, 3), allFlows[i],0);
                var flowOnMap = flowMap.GetFlow((i, 3));
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
