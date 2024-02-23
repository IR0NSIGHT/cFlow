using application.Maps;
using application.Maps.flowMap;
using cFlowApi.Heightmap;
using src.Maps.riverMap;


namespace unittest
{
    [TestFixture]
    public class FloodToolTest
    {
        private IEnumerable<(int x, int y)> iterateRect((int x, int y) start, (int x, int y) end)
        {
            for (int x = start.x; x < end.x; x++)
            {
                for (int y = start.y; y < end.y; y++)
                {
                    yield return (x, y);
                }
            }
        }

        private bool isInRect((int x, int y) point, (int x, int y) start, (int x, int y) end)
        {
            return point.x >= start.x && point.y >= start.y && point.x < end.x && point.y < end.y;
        }

        [Test]
        public void FloodConfinedHoleUpTo()
        {
            DummyDimension heightMap = new DummyDimension((5, 7), 74);
            (int x, int y) rectStart = (1, 2);
            (int x, int y) rectEnd = (5, 5);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            uint[,] shouldBeHeight = new uint[,]
            {
                { 74, 74, 74, 74, 74 },
                { 74, 74, 74, 74, 74 },
                { 74, 17, 17, 17, 17 },
                { 74, 17, 17, 17, 17 },
                { 74, 17, 17, 17, 17 },
                { 74, 74, 74, 74, 74 },
                { 74, 74, 74, 74, 74 },

            };
            Assert.That(heightMap.ToGPUdata(), Is.EqualTo(shouldBeHeight));


            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (outerMostRing, seen17, exceeded, escapePoints) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (2, 3) },
                17, p => false
                );

            Assert.That(exceeded, Is.False);

            bool[,] shouldBeFlooded = new bool[,]
            {
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, true,  true,  true, true },
                { false, true,  true,  true, true },
                { false, true,  true,  true, true },
                { false, false, false, false, false },
                { false, false, false, false, false },
            };
            Assert.That(seen17.ToGpuData(), Is.EqualTo(shouldBeFlooded));


            bool[,] shouldBeRing = new bool[,]
            {
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, true,  true,  true,  true },
                { false, true,  false, false, false },
                { false, true,  true,  true,  true },
                { false, false, false, false, false },
                { false, false, false, false, false },
            };

            foreach (var point in heightMap.iterator().Points())
            {
                var supposedToBeRing = shouldBeRing[point.y, point.x];
                if (supposedToBeRing)
                    Assert.That(outerMostRing.Contains(point));
                else
                    Assert.That(outerMostRing.Contains(point), Is.False);
            }
        }

        [Test]
        public void FloodConfinedHoleUpToSomeHeight()
        {
            /** high/low
            *   h h h h
            *   h l l h
            *   h h h h
            */


            IHeightMap heightMap = new DummyDimension((50, 50), 74);
            (int x, int y) rectStart = (10, 20);
            (int x, int y) rectEnd = (30, 40);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (ring, seen17, exceeded, escapePoints) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 30) },
                34,  //not z of bottom plain, but higher but still below rest of map,
                p => false
                );

            Assert.That(exceeded, Is.False);

            //test that points were marked correctly as "plain" or "not plain"
            foreach (var point in heightMap.iterator().Points())
            {
                if (isInRect(point, rectStart, rectEnd))
                {
                    Assert.That(seen17.isMarked(point.x, point.y), Is.True, $"point {point} is marked wrong");
                }
                else
                {
                    Assert.That(seen17.isMarked(point.x, point.y), Is.False, $"point {point} is marked wrong");
                }
            }


            //test outmost points were correctly collected
            for (var x = rectStart.x; x < rectEnd.x; x++)
            {
                Assert.That(ring.Contains((x, rectStart.y)), Is.True, $"point {(x, rectStart.y)} is marked wrong");
                Assert.That(ring.Contains((x, rectEnd.y - 1)), Is.True, $"point {(x, rectEnd.y - 1)} is marked wrong");
            }

            for (var y = rectStart.y; y < rectEnd.y; y++)
            {
                Assert.That(ring.Contains((rectStart.x, y)), Is.True);
                Assert.That(ring.Contains((rectEnd.x - 1, y)), Is.True);
            }


        }

        [Test]
        public void FloodCompleteMap()
        {
            /** high/low
            *   h h h h
            *   h l l h
            *   h h h h
            */


            IHeightMap heightMap = new DummyDimension((50, 50), 74);
            (int x, int y) rectStart = (10, 20);
            (int x, int y) rectEnd = (30, 40);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (ring, seenMap, exceeded, escapePoints) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (0, 0) },
                74, //map is all 74 or lower => flood all
                p => false
                );

            Assert.That(exceeded, Is.False);
            //all points on map were flooded
            foreach (var point in heightMap.iterator().Points())
            {
                Assert.That(seenMap.isMarked(point.x, point.y), Is.True, $"point {point} is marked wrong");
            }

            //no border exists, map is flooded
            Assert.That(ring.Count, Is.EqualTo(0));

            Assert.That(exceeded, Is.False);

        }

        [Test]
        public void AbortWhenFindingHole()
        {
            /** high/low
            *   h h h h
            *   h l l h
            *   h h h h
            */


            IHeightMap heightMap = new DummyDimension((50, 50), 74);
            (int x, int y) rectStart = (10, 20);
            (int x, int y) rectEnd = (30, 40);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (ring, seenMap, exceeded, escapePoints) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (0, 0) },
                74, //map is all 74 or lower => flood all
                p => false,
                -1,
                true
            );

            Assert.That(exceeded, Is.False);
            Assert.That(escapePoints.Count != 0);
            //all points on map were flooded
            foreach (var point in escapePoints)
            {
                Assert.That(isInRect(point, rectStart, rectEnd), Is.True, $"point {point} is marked wrong");
            }
        }


        [Test]
        public void AbortWhenBorderExceeded()
        {
            /** high/low
            *   h h h h
            *   h l l h
            *   h h h h
            */


            IHeightMap heightMap = new DummyDimension((50, 50), 74);
            (int x, int y) rectStart = (10, 20);
            (int x, int y) rectEnd = (30, 40);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (ring, seenMap, exceeded, escapePoints) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 25) },
                17,
                p => false,
                13
                );

            Assert.That(exceeded, Is.True);
        }

        [Test]
        public void ExceededMultiLevelLake()
        {
            /** high/medium/low map sketch
            *   h h h h h
            *   h m m m h
            *   h m l m h
             *  h m m m h
             *  h h h h h
            */


            IHeightMap heightMap = new DummyDimension((1000, 1000), 74);
            (int x, int y) rectStart = (30, 20);
            (int x, int y) rectEnd = (40, 30);

            //make a large hole: 100x100 size
            foreach (var point in iterateRect((10, 10), (110, 110)))
            {
                heightMap.SetHeight(point, 25);
            }

            //make hole in the middle: 10x10 size
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);



            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var riverMap = new RiverMap(new DistanceMap(heightMap), heightMap);
            flood.FloodArea(
                (35, 25),
                riverMap,
                100,
                500 //big enough for small hole, to small for middle hole
                );

            bool IsInHole((int x, int y) p) => isInRect(p, rectStart, rectEnd);

            //check that map was correctly marked
            foreach (var point in heightMap.iterator().Points())
            {
                if (IsInHole(point))
                {
                    Assert.That(riverMap.IsRiver(point.x, point.y), Is.True);
                }
                else
                {
                    Assert.That(riverMap.IsRiver(point.x, point.y), Is.False, $"incorreclty marked point: {point}");
                }
            }

            bool IsHoleEdge((int x, int y) p) => IsInHole(p) && !isInRect(p, (rectStart.x + 1, rectStart.y + 1), (rectEnd.x - 1, rectEnd.y - 1));
        }
    }
}
