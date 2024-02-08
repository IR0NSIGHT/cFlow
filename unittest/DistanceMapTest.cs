using application.Maps.flowMap;

namespace unittest;

[TestFixture]
public class DistanceMapTest
{
    [Test]
    public void CollectNextIteration()
    {

        var dMap = new DistanceMap(new DummyDimension((10, 10), 0));
        var origins = new List<((int, int) point, DistanceMap.DistancePoint distance)>
        {
            ((3, 5), new DistanceMap.DistancePoint(0,0, true)),
            ((7, 8), new DistanceMap.DistancePoint(0,0, true))
        };
        dMap.SetDistanceToEdge(origins[0].point, origins[0].distance);
        dMap.SetDistanceToEdge(origins[1].point, origins[1].distance);

        var firstLayer = dMap.ExpandDistancesFor(origins);
        var shouldBe = new List<(int, int)>();
        shouldBe.AddRange(Point.Neighbours((3, 5)));
        shouldBe.AddRange(Point.Neighbours((7, 8)));
        foreach (var tuple in firstLayer)
        {
            Assert.That(shouldBe.Contains(tuple.point), Is.True, $"point {tuple} not in list");
            Assert.That(tuple.distance.DistanceSquared, Is.EqualTo(1));
        }

        var secondLayer = dMap.ExpandDistancesFor(firstLayer);
        foreach (var tuple in secondLayer)
        {
            Assert.That(origins.Contains(tuple), Is.False, $"point was logged twice {tuple}");
            Assert.That(shouldBe.Contains(tuple.point), Is.False, $"point {tuple} logged twice");
            Assert.That(tuple.distance.DistanceSquared, Is.EqualTo(2).Or.EqualTo(4));
        }
    }

    [Test]
    public void ApplyFromHeightmap()
    {
        var hMap = new DummyDimension((5, 5), 7);
        hMap.SetHeight((2, 2), 4);
        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        short[][] shouldBe =
        {
            [5, 2, 1, 2, 5],
            [2, 1, 0, 1, 2],
            [1, 0, 0, 0, 1],    //hole in middle at  2,2
            [2, 1, 0, 1, 2],
            [5, 2, 1, 2, 5]
        };
        Assert.That(dMap.IsSet((2, 2)), Is.False);
        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.DistanceSquared, Is.EqualTo(shouldBe[point.y][point.x]));
        }
    }

    [Test]
    public void ApplyFromHeightmapHole2x2()
    {
        var hMap = new DummyDimension((5, 5), 7);
        hMap.SetHeight((0, 0), 4);
        hMap.SetHeight((1, 0), 4);

        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        short[][] shouldBe =
        {
            [0, 0, 0, 1, 4],//hole in top left at 0,0 and 1,0
            [0, 0, 1, 2, 5],
            [1, 1, 2, 5, 8],
            [4, 4, 5, 8, 13],
            [9, 9, 10, 13, 18]
        };
        Assert.That(dMap.IsSet((0, 0)), Is.False);
        Assert.That(dMap.IsSet((1, 0)), Is.False);

        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.DistanceSquared, Is.EqualTo(shouldBe[point.y][point.x]));
        }
    }
}