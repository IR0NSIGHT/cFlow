using System.Diagnostics;
using application.Maps.flowMap;
using cFlowApi.Heightmap;
using cFlowAPI.Maps.Shader;
using ComputeSharp;

namespace unittest;

[TestFixture]
public class DistanceMapTest
{
    [Test]
    public void ApplyFromHeightmap()
    {
        var hMap = new DummyDimension((6, 5), 7);
        hMap.SetHeight((2, 2), 4);
        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        short[][] shouldBe =
        {
            [34, 24, 20, 24, 34,38],
            [24, 20, 10, 20, 24,34],
            [20, 10,  0, 10, 20,30],    //hole in middle at  24,24
            [24, 20, 10, 20, 24,34],
            [34, 24, 20, 24, 34,38]
        };
        Assert.That(dMap.IsSet((2, 2)), Is.False);

        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.distance, Is.EqualTo(shouldBe[point.y][point.x]));
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
            [ 0,  0, 10, 20, 30],//hole in top left at 0,0 and 10,0
            [10, 10, 20, 24, 34],
            [20, 20, 24, 34, 38],
            [30, 30, 34, 38, 48],
            [40, 40, 44, 48, 52]
        };
        Assert.That(dMap.IsSet((0, 0)), Is.False);
        Assert.That(dMap.IsSet((1, 0)), Is.False);

        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.distance, Is.EqualTo(shouldBe[point.y][point.x]));
        }
    }


    private static ExpandDistanceShader FromMaps(uint[,] distanceMap, uint[,] heightMap)
    {
        var distanceData = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(distanceMap);

        var heightTexture = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D(heightMap);

        var changedArr = new int[1];
        var changed = GraphicsDevice.GetDefault()
            .AllocateReadWriteBuffer<int>(changedArr);

        return new ExpandDistanceShader(distanceData, heightTexture, changed); ;
    }

    public static bool didChange(ExpandDistanceShader shader)
    {
        var result = new int[1];
        shader.changed.CopyTo(result);
        return result[0] != 0;
    }

    [Test]
    public void simpleShaderRun()
    {
        DummyDimension dimension = new DummyDimension((5, 7), 17);
        DistanceMap distanceMap = new DistanceMap(dimension);
        for (int x = 0; x < dimension.Bounds().x; x++)
            distanceMap.SetDistanceToEdge((x, 2), new DistanceMap.DistancePoint(10, true));


        uint[,] pointData = distanceMap.toGpuData();
        uint[,] heightData = dimension.ToGPUdata();

        Assert.That(pointData.GetLength(0) == heightData.GetLength(0));
        Assert.That(pointData.GetLength(1) == heightData.GetLength(1));



        //check before first run
        var shouldBe = new uint[,]
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 10, 10, 10, 10, 10, },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));

        var shader = FromMaps(pointData, heightData);

        //first run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);

        shouldBe = new uint[,]
        {
            { 0, 0, 0, 0, 0 },
            { 20, 20, 20, 20, 20, },
            { 10, 10, 10, 10, 10, },
            { 20, 20, 20, 20, 20, },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //second run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 30, 30, 30, 30, 30 },
            { 20, 20, 20, 20, 20, },
            { 10, 10, 10, 10, 10, },
            { 20, 20, 20, 20, 20, },
            { 30, 30, 30, 30, 30 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //third run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 30, 30, 30, 30, 30 },
            { 20, 20, 20, 20, 20, },
            { 10, 10, 10, 10, 10, },
            { 20, 20, 20, 20, 20, },
            { 30, 30, 30, 30, 30 },
            { 40, 40, 40, 40, 40 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //forth run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 30, 30, 30, 30, 30 },
            { 20, 20, 20, 20, 20, },
            { 10, 10, 10, 10, 10, },
            { 20, 20, 20, 20, 20, },
            { 30, 30, 30, 30, 30 },
            { 40, 40, 40, 40, 40 },
            { 50, 50, 50, 50, 50 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //fifth and last run => no change
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 30, 30, 30, 30, 30 },
            { 20, 20, 20, 20, 20, },
            { 10, 10, 10, 10, 10, },
            { 20, 20, 20, 20, 20, },
            { 30, 30, 30, 30, 30 },
            { 40, 40, 40, 40, 40 },
            { 50, 50, 50, 50, 50 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader), Is.False);

    }

    public void print(uint[,] pointData)
    {
        for (int y = 0; y < pointData.GetLength(0); y++)
        {
            for (int x = 0; x < pointData.GetLength(1); x++)
            {
                Debug.Write(pointData[y,x] +",\t");
            }
            Debug.Write("\n");
        }
    }

    [Test]
    public void simpleShaderRunDiagonal()
    {
        DummyDimension dimension = new DummyDimension((5, 7), 17);
        DistanceMap distanceMap = new DistanceMap(dimension);

        distanceMap.SetDistanceToEdge((2, 2), new DistanceMap.DistancePoint(10, true));

        uint[,] pointData = distanceMap.toGpuData();
        uint[,] heightData = dimension.ToGPUdata();

        Assert.That(pointData.GetLength(0) == heightData.GetLength(0));
        Assert.That(pointData.GetLength(1) == heightData.GetLength(1));



        //check before first run
        var shouldBe = new uint[,]
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 10, 0, 0, },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));

        var shader = FromMaps(pointData, heightData);

        //first run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);

        shouldBe = new uint[,]
        {
            { 0, 0, 0, 0, 0 },
            { 0, 24, 20, 24, 0 },
            { 0, 20, 10, 20, 0, },
            { 0, 24, 20, 24, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //second run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 38,  34, 30, 34, 38 },
            { 34, 24, 20, 24, 34 },
            { 30, 20, 10, 20, 30, },
            { 34, 24, 20, 24, 34 },
            { 38, 34, 30, 34, 38 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
        print(pointData);
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //third run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 38,  34, 30, 34, 38 },
            { 34, 24, 20, 24, 34 },
            { 30, 20, 10, 20, 30, },
            { 34, 24, 20, 24, 34 },
            { 38, 34, 30, 34, 38 },
            { 48, 44, 40, 44, 48 },
            { 0, 0, 0, 0, 0 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //forth run
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 38,  34, 30, 34, 38 },
            { 34, 24, 20, 24, 34 },
            { 30, 20, 10, 20, 30, },
            { 34, 24, 20, 24, 34 },
            { 38, 34, 30, 34, 38 },
            { 48, 44, 40, 44, 48 },
            { 58, 54, 50, 54, 58 },
        };

        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader));

        //fifth and last run => no change
        shader.changed.CopyFrom(new int[1]);
        GraphicsDevice.GetDefault().For(dimension.Bounds().x, dimension.Bounds().y, shader);

        shader.distanceMap.CopyTo(pointData);


        shouldBe = new uint[,]
        {
            { 38,  34, 30, 34, 38 },
            { 34, 24, 20, 24, 34 },
            { 30, 20, 10, 20, 30, },
            { 34, 24, 20, 24, 34 },
            { 38, 34, 30, 34, 38 },
            { 48, 44, 40, 44, 48 },
            { 58, 54, 50, 54, 58 },
        };
        Assert.That(pointData, Is.EqualTo(shouldBe));
        Assert.That(didChange(shader), Is.False);

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
            [0,  10, 20, 30, 40],
            [10, 20, 24, 34, 30],
            [20, 24, 34, 24, 20],
            [30, 34, 24, 20, 10],
            [40, 30, 20, 10,  0]
        };
        Assert.That(dMap.IsSet((0, 0)), Is.False);
        Assert.That(dMap.IsSet((4, 4)), Is.False);

        foreach (var point in dMap.iterator().Points())
        {
            var value = dMap.GetDistanceOf(point);
            Assert.That(value.DistanceSquared, Is.EqualTo(shouldBe[point.y][point.x]));
        }
    }

    [Test]
    public void LargeDistances()
    {
        var hMap = new DummyDimension((1001, 1001), 7);
        hMap.SetHeight((0, 1000), 4);

        var dMap = new DistanceMap(hMap);
        dMap.CalculateFromHeightmap();

        Assert.That(dMap.IsSet((0, 1000)), Is.False);
        foreach (var point in dMap.iterator().Points())
        {
            if (point == (0, 1000))
                continue;
            Assert.That(dMap.GetDistanceOf(point).isSet, Is.True, "undefined point after whole map should have a value");
        }

        var edge = dMap.GetDistanceOf((1000, 0));
        Assert.That(edge.distance, Is.EqualTo(20 + (14 * 999)));

    }
}