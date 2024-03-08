using application.Maps;
using application.Maps.flowMap;
using cFlowApi.Heightmap;
using cFlowAPI.Maps.Shader;
using ComputeSharp;
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
            var floodPlane = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (2, 3) },
                17, p => false
                );

            Assert.That(floodPlane.exceededMaxSurface, Is.False);

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
            Assert.That(floodPlane.planeMap.ToGpuData(), Is.EqualTo(shouldBeFlooded));


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
                    Assert.That(floodPlane.borderRing.Contains(point));
                else
                    Assert.That(floodPlane.borderRing.Contains(point), Is.False);
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
            var floodPlane = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 30) },
                34,  //not z of bottom plain, but higher but still below rest of map,
                p => false
                );

            Assert.That(floodPlane.exceededMaxSurface, Is.False);

            //test that points were marked correctly as "plain" or "not plain"
            foreach (var point in heightMap.iterator().Points())
            {
                if (isInRect(point, rectStart, rectEnd))
                {
                    Assert.That(floodPlane.planeMap.isMarked(point.x, point.y), Is.True, $"point {point} is marked wrong");
                }
                else
                {
                    Assert.That(floodPlane.planeMap.isMarked(point.x, point.y), Is.False, $"point {point} is marked wrong");
                }
            }


            //test outmost points were correctly collected
            for (var x = rectStart.x; x < rectEnd.x; x++)
            {
                Assert.That(floodPlane.borderRing.Contains((x, rectStart.y)), Is.True, $"point {(x, rectStart.y)} is marked wrong");
                Assert.That(floodPlane.borderRing.Contains((x, rectEnd.y - 1)), Is.True, $"point {(x, rectEnd.y - 1)} is marked wrong");
            }

            for (var y = rectStart.y; y < rectEnd.y; y++)
            {
                Assert.That(floodPlane.borderRing.Contains((rectStart.x, y)), Is.True);
                Assert.That(floodPlane.borderRing.Contains((rectEnd.x - 1, y)), Is.True);
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
            var floodPlane = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (0, 0) },
                74, //map is all 74 or lower => flood all
                p => false
                );

            Assert.That(floodPlane.exceededMaxSurface, Is.False);
            //all points on map were flooded
            foreach (var point in heightMap.iterator().Points())
            {
                Assert.That(floodPlane.planeMap.isMarked(point.x, point.y), Is.True, $"point {point} is marked wrong");
            }

            //no border exists, map is flooded
            Assert.That(floodPlane.borderRing.Count, Is.EqualTo(0));

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
            var floodPlane = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (0, 0) },
                74, //map is all 74 or lower => flood all
                p => false,
                -1,
                true
            );

            Assert.That(floodPlane.exceededMaxSurface, Is.False);
            Assert.That(floodPlane.escapePoints.Count != 0);
            //all points on map were flooded
            foreach (var point in floodPlane.escapePoints)
            {
                Assert.That(isInRect(point, rectStart, rectEnd), Is.True, $"point {point} is marked wrong");
            }
        }

        [Test]
        public void CanEscapeHole()
        {
            /** high/low
            *   h h h h
            *   h l l h
            *   h h h h
            */


            DummyDimension heightMap = new DummyDimension((5, 4), 7);
            heightMap.FromGPUdata(new int[,] {
                { 7, 7, 7, 7, 7},
                { 7, 3, 4, 7, 3},
                { 7, 7, 7, 7, 7},
                { 7, 1, 2, 6, 6}
            });
            RiverMap riverMap = new RiverMap(new DistanceMap(heightMap), heightMap);


            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var escapePoints = flood.FloodArea((1, 1), riverMap, new BooleanMap(riverMap.Bounds()),10,100);
            escapePoints.Sort();

            var shouldBeEscapePoints = new List<(int x, int y)>() { (4, 1), (1, 3), (2, 3) };
            shouldBeEscapePoints.Sort();

            Assert.That(escapePoints, Is.EqualTo(shouldBeEscapePoints));

        }

        [Test]
        public void CanEscapeHoleWideBottom()
        {
            DummyDimension heightMap = new DummyDimension((5, 4), 7);
            heightMap.FromGPUdata(new int[,] {
                { 7, 7, 7, 7, 7},
                { 7, 3, 3, 7, 3},
                { 7, 3, 3, 7, 7},
                { 7, 3, 3, 6, 6}
            });
            RiverMap riverMap = new RiverMap(new DistanceMap(heightMap), heightMap);


            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var escapePoints = flood.FloodArea((1, 1), riverMap, new BooleanMap(riverMap.Bounds()), 10, 100);
            escapePoints.Sort();

            var shouldBeEscapePoints = new List<(int x, int y)>() { (4, 1) };
            shouldBeEscapePoints.Sort();

            Assert.That(escapePoints, Is.EqualTo(shouldBeEscapePoints));

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
            var floodPlane = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 25) },
                17,
                p => false,
                13
                );

            Assert.That(floodPlane.exceededMaxSurface, Is.True);
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
                new BooleanMap(riverMap.Bounds()),
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
        }

        [Test]
        public void LargeAreaFloodPerformanceDebugger()
        {
            DummyDimension heightMap = new DummyDimension((10000, 10000), 74);

            var riverMap = new RiverMap(new DistanceMap(heightMap), heightMap);

            (int x, int y) rectStart = (50, 100);
            (int x, int y) rectEnd = (150, 200);
            //make a large hole: 100x100 size
            foreach (var point in iterateRect(rectStart, rectEnd))
            {
                heightMap.SetHeight(point, 25);
            }

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            flood.FloodArea(
                (35, 25),
                riverMap,
                new BooleanMap(riverMap.Bounds()),
                100,
                100000
                );
        }
    }
}
