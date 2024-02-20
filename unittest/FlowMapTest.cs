using cFlowApi.Heightmap;

namespace tests
{
    [TestFixture]
    public class FlowMapTest
    {
        [Test]
        public void BasicFlowMap()
        {
            var flowMap = new SimpleFlowMap((10, 20));
            var flow = new IFlowMap.Flow(true, false, false, false, false);
            flowMap.SetFlow((5, 7), flow);
            Assert.That(flowMap.GetFlow((5, 7)), Is.EqualTo(flow));

            flow = new IFlowMap.Flow(false, true, false, true, false);
            flowMap.SetFlow((5, 7), flow);
            Assert.That(flowMap.GetFlow((5, 7)), Is.EqualTo(flow));

            flow = new IFlowMap.Flow(false, false, true, false, true);
            flowMap.SetFlow((5, 7), flow);
            Assert.That(flowMap.GetFlow((5, 7)), Is.EqualTo(flow));

            flow = new IFlowMap.Flow(false, true, true, true, true);
            flowMap.SetFlow((5, 7), flow);
            Assert.That(flowMap.GetFlow((5, 7)), Is.EqualTo(flow));
        }



        //[Test] //FIXME suspected to lead to issues with test runner for some reason?
        public void TestMapOrientation()
        {
            var dims = (10, 3);
            var fMap = new SimpleFlowMap(dims);

            //mark y = 0 ==> first column 

            fMap.SetFlow((0, 0), new IFlowMap.Flow(false, true, false, false, false));
            Assert.IsTrue(fMap.GetFlow((0, 0)).Up);
            fMap.SetFlow((9, 0), new IFlowMap.Flow(false, true, false, false, false));
            Assert.IsTrue(fMap.GetFlow((9, 0)).Up);

            //picture origin is top left = 0,0
            //width = x
            //height = y


            fMap.SetFlow((9, 2), new IFlowMap.Flow(false, false, true, false, false));
            Assert.IsTrue(fMap.GetFlow((9, 2)).Down);

            var bitmap = SimpleFlowMap.ToColorImage(fMap, FlowTranslation.FlowToColor);
            //ImageApi.SaveBitmapAsPng(bitmap, "C:\\Users\\Max1M\\OneDrive\\Bilder\\debug_flowmap.png");

        }

        [Test]
        public void NaturalFlow()
        {
            //test if depressresions in the terrain will cause neighboursing blocks to be marked as natural edge flow

            var heightmap = new DummyDimension((10, 3), 17);
            heightmap.SetHeight((0, 0), 5);

            var flowMap = new SimpleFlowMap(heightmap.Bounds());
            SimpleFlowMap.ApplyNaturalEdgeFlow(heightmap, flowMap);
            foreach (var point in flowMap.iterator().Points())
            {
                var (x, y) = (point.x, point.y);
                if ((x, y) == (0, 1))
                {
                    Assert.AreEqual(new IFlowMap.Flow(false, false, true, false, false), flowMap.GetFlow(point));
                }
                else if ((x, y) == (1, 0))
                {
                    Assert.AreEqual(new IFlowMap.Flow(false, false, false, true, false), flowMap.GetFlow(point));
                }
                else
                {
                    Assert.AreEqual(new IFlowMap.Flow(true, false, false, false, false), flowMap.GetFlow(point));
                }
            }
        }

        [Test]
        public void TestEdgesNaturalFlow()
        {
            var heightMap = new DummyDimension((30, 40), 17);
            //make rectangle from 10..20, 15..25 that is raised => water will flow away from rectangle
            for (int x = 10; x <= 20; x++)
            {
                for (int y = 15; y <= 25; y++)
                {
                    heightMap.SetHeight((x, y), 37);
                }
            }
            var flowMap = new SimpleFlowMap((30, 40));
            SimpleFlowMap.ApplyNaturalEdgeFlow(heightMap, flowMap);
            //check edges
            Assert.That(flowMap.GetFlow((10, 15)), Is.EqualTo(new IFlowMap.Flow(false, false, true, true, false)));
            Assert.That(flowMap.GetFlow((20, 15)), Is.EqualTo(new IFlowMap.Flow(false, false, true, false, true)));
            Assert.That(flowMap.GetFlow((10, 25)), Is.EqualTo(new IFlowMap.Flow(false, true, false, true, false)));
            Assert.That(flowMap.GetFlow((20, 25)), Is.EqualTo(new IFlowMap.Flow(false, true, false, false, true)));

            //top and bottom lines
            for (int x = 11; x < 20; x++)
            {
                Assert.That(flowMap.GetFlow((x, 15)), Is.EqualTo(new IFlowMap.Flow(false, false, true, false, false)));
                Assert.That(flowMap.GetFlow((x, 25)), Is.EqualTo(new IFlowMap.Flow(false, true, false, false, false)));
            }

            //left right lines
            for (int y = 16; y < 25; y++)
            {
                Assert.That(flowMap.GetFlow((10, y)), Is.EqualTo(new IFlowMap.Flow(false, false, false, true, false)));
                Assert.That(flowMap.GetFlow((20, y)), Is.EqualTo(new IFlowMap.Flow(false, false, false, false, true)));
            }
        }

        [Test]
        public void MostBasicNaturalFlow()
        {
            var flowMap = new SimpleFlowMap((6, 12));
            var heightMap = new DummyDimension(flowMap.Bounds(), 17);
            heightMap.SetHeight((2, 3), 5);

            SimpleFlowMap.ApplyNaturalEdgeFlow(heightMap, flowMap);

            Assert.That(flowMap.GetFlow((2, 3)).Unknown);
            Assert.That(flowMap.GetFlow((1, 3)),
                Is.EqualTo(new IFlowMap.Flow(false, false, false, false, true)));

            Assert.That(flowMap.GetFlow((3, 3)),
                Is.EqualTo(new IFlowMap.Flow(false, false, false, true, false)));

            Assert.That(flowMap.GetFlow((2, 2)),
                Is.EqualTo(new IFlowMap.Flow(false, true, false, false, false)));

            Assert.That(flowMap.GetFlow((2, 4)),
                Is.EqualTo(new IFlowMap.Flow(false, false, true, false, false)));
        }

        [Test]
        public void OneDirectionalNaturalFlow()
        {
            var flowMap = new SimpleFlowMap((6, 12));
            var heightMap = new DummyDimension(flowMap.Bounds(), 17);
            for (int x = 0; x < 6; x++)
            {
                heightMap.SetHeight((x, 3), 5);
            }

            SimpleFlowMap.ApplyNaturalEdgeFlow(heightMap, flowMap);

            foreach (var point in flowMap.iterator().Points())
            {
                if (point.y == 2)
                {
                    Assert.That(flowMap.GetFlow(point),
                    Is.EqualTo(new IFlowMap.Flow(false, true, false, false, false)));
                }
                else if (point.y == 4)
                {
                    Assert.That(flowMap.GetFlow(point),
                        Is.EqualTo(new IFlowMap.Flow(false, false, true, false, false)));
                }
                else
                {
                    Assert.That(flowMap.GetFlow(point),
                        Is.EqualTo(new IFlowMap.Flow(true, false, false, false, false)));

                }
            }
        }

        [Test]
        public void FlowFromHeightMapSimple()
        {
            var flowMap = new SimpleFlowMap((6, 12));
            var someFlow = new IFlowMap.Flow(false, true, false, false, false);
            var heightMap = new DummyDimension(flowMap.Bounds(), 17);
            //all but a 4x4 square in the center has a flow
            for (int x = 1; x < 5; x++)
            {
                for (int y = 1; y < 7; y++)
                {
                    heightMap.SetHeight((x, y), 77);
                }
            }

            foreach (var point in flowMap.iterator().Points())
            {
                Assert.That(flowMap.GetFlow(point).Unknown, Is.True);
            }

            SimpleFlowMap.CalculateFlowFromHeightMap(heightMap, flowMap);


            Func<(int, int), Boolean> insideMountainRect = ((int x, int y) pos) =>
            {
                if (pos.x >= 1 && pos.x < 5 && pos.y >= 1 && pos.y < 7)
                    return true;
                return false;
            };

            foreach (var point in flowMap.iterator().Points())
            {
                if (insideMountainRect((point.x, point.y)))
                {
                    Assert.That(flowMap.GetFlow(point).Unknown, Is.Not.True,$"incorrect known marked: {point},{flowMap.GetFlow(point)}");
                }
                else
                {
                    Assert.That(flowMap.GetFlow(point).Unknown, Is.True, $"incorrect known marked: {point},{flowMap.GetFlow(point)}");
                }
            }

            var downLeft = new IFlowMap.Flow(false, false, true, true, false);
            Assert.That(flowMap.GetFlow((1, 1)), Is.EqualTo(downLeft));
            Assert.That(flowMap.GetFlow((2, 2)), Is.EqualTo(downLeft));

            var downRight = new IFlowMap.Flow(false, false, true, false, true);
            Assert.That(flowMap.GetFlow((4, 1)), Is.EqualTo(downRight));
            Assert.That(flowMap.GetFlow((3, 2)), Is.EqualTo(downRight));


            var upLeft = new IFlowMap.Flow(false, true, false, true, false);
            Assert.That(flowMap.GetFlow((1, 6)), Is.EqualTo(upLeft));
            Assert.That(flowMap.GetFlow((2, 5)), Is.EqualTo(upLeft));

            var upRight = new IFlowMap.Flow(false, true, false, false, true);
            Assert.That(flowMap.GetFlow((4, 6)), Is.EqualTo(upRight));
            Assert.That(flowMap.GetFlow((3, 5)), Is.EqualTo(upRight));

        }
    }
}
