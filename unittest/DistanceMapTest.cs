using application.Maps.flowMap;

namespace unittest;

[TestFixture]
public class DistanceMapTest
{
    [Test]
    public void ApplyFromHeightmap()
    {
        var hMap = new DummyDimension((5, 5), 7);
        hMap.SetHeight((2, 2), 4);
        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        short[][] shouldBe =
        {
            [24, 14, 10, 14, 24],
            [14, 10, 0, 10, 14],
            [10, 0, 0, 0, 10],    //hole in middle at  14,14
            [14, 10, 0, 10, 14],
            [24, 14, 10, 14, 24]
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

    [Test]
    public void HeightmapWithTwoSmallHoles()
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
    public void MultipleHoles()
    {
        var hMap = new DummyDimension((5, 5), 7);
        hMap.SetHeight((0, 0), 4);
        hMap.SetHeight((4, 4), 4);

        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        short[][] shouldBe =
        {
            [0, 0, 1, 4, 9],
            [0, 1, 2, 5, 4],
            [1, 2, 5,2, 1],
            [4, 5, 2, 1, 0],
            [9, 4, 1, 0, 0]
        };
        Assert.That(dMap.IsSet((0, 0)), Is.False);
        Assert.That(dMap.IsSet((4,4)), Is.False);

        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.DistanceSquared, Is.EqualTo(shouldBe[point.y][point.x]));
        }
    }

    [Test]
    public void LargeDistances()
    {
        var hMap = new DummyDimension((1001,1001), 7);
        hMap.SetHeight((0,1000), 4);

        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        Assert.That(dMap.IsSet((0, 1000)), Is.False);
        foreach (var point in dMap.iterator().Points())
        {
            if (point == (0,1000))
                continue;
            Assert.That(dMap.GetDistanceOf(point).isSet, Is.True, "undefined point after whole map should have a value");
        }

        var edge = dMap.GetDistanceOf((1000, 0));
        Assert.That(edge.DistanceSquared, Is.EqualTo(1000*1000+999*999));

    }
}