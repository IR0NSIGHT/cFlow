using System.Drawing;
using static FlowTranslation;
namespace namespace_01
{
    [TestFixture]
    public class FlowColorTest
    {
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
            }
        }
    }
}
