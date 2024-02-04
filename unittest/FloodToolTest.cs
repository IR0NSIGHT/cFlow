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
            IHeightMap heightMap = new DummyDimension((50, 50), 74);
            (int x, int y) rectStart = (10, 20);
            (int x, int y) rectEnd = (30, 40);
            //make hole in the middle
            foreach (var point in iterateRect(rectStart, rectEnd))
                heightMap.SetHeight(point, 17);

            var flood = new cFlowAPI.Maps.riverMap.FloodTool(heightMap);
            var (ring, seen17, exceeded) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 30) },
                17
                );

            Assert.That(exceeded, Is.False);

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
            var (ring, seen17, exceeded) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25, 30) },
                34  //not z of bottom plain, but higher but still below rest of map
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
            var (ring, seenMap, exceeded) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (0,0) },
                74  //map is all 74 or lower => flood all
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
            var (ring, seenMap, exceeded) = flood.collectPlaneAtOrBelow(
                new List<(int x, int y)> { (25,25) },
                74,  
                13
                );

            Assert.That(exceeded, Is.True);
        }
    }
}
