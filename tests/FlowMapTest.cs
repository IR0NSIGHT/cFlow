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
        
        }
    }
}
