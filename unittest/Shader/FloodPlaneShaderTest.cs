using application.Maps;
using cFlowApi.Heightmap;
using cFlowAPI.Maps.Shader;
using ComputeSharp;

namespace unittest.Shader;

[TestFixture]
public class FloodPlaneShaderTest
{
    [Test]
    public void FloodOneLevelAuto()
    {
        DummyDimension heightMap = new DummyDimension((5, 4), 7);
        heightMap.FromGPUdata(new int[,] {
                { 7, 7, 7, 7, 7},
                { 7, 3, 3, 7, 3},
                { 4, 3, 3, 4, 7},
                { 5, 3, 3, 6, 6}
            });
        BooleanMap booleanMap = new BooleanMap(heightMap.Bounds());
        booleanMap.setMarked(1, 2);

        //check before running
        var shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
        };
        var flooded = booleanMap.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));

        //Execute shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        FloodPlaneComputer.RunUntilEscapeFoundOrPlaneDone(shader);

        //verify first iteration
        flooded = FloodPlaneComputer.MarkedMapFromShader(shader).ToGpuData();
        shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 1, 0, 0 },
                { 0, 1, 1, 0, 0 },
                { 0, 1, 1, 0, 0 }
        };
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));

    }

    [Test]
    public void FindsClosestEscapePoints()
    {
        DummyDimension heightMap = new DummyDimension((5, 4), 7);
        heightMap.FromGPUdata(new int[,] {
                { 1, 7, 7, 1, 7},
                { 7, 3, 3, 1, 3},/*B*/
         /*A*/  { 1, 3, 3, 7, 7},
                { 7, 3, 3, 1, 7} /*C*/
            });
        BooleanMap booleanMap = new BooleanMap(heightMap.Bounds());
        booleanMap.setMarked(1, 1);

        //check before running
        var shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
        };
        var flooded = booleanMap.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));


        //run flood shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        FloodPlaneComputer.RunUntilEscapeFoundOrPlaneDone(shader);

        var idx = shader.escapeIdx.ToArray();
        var escapePoints = Array.ConvertAll(shader.escapePoints.ToArray(), new Converter<int2, (int x, int y)>(i2 => (i2.X, i2.Y)));

        //A and B are found, C is not reached because the shader aborts earlier.
        CollectionAssert.AreEquivalent(
            new (int x, int y)[] { (0, 2), (3, 1) },
            FloodPlaneComputer.GetFoundEscapePoints(shader)
            );

    }


    [Test]
    public void IgnoresMarkedPointAsEscapes()
    {
        DummyDimension heightMap = new DummyDimension((5, 4), 7);
        heightMap.FromGPUdata(new int[,] {
                { 1, 7, 7, 7, 7},
                { 7, 3, 3, 3, 3},
                { 3, 3, 2, 2, 7},
                { 7, 3, 3, 3, 7}
            });
        BooleanMap booleanMap = new BooleanMap(heightMap.Bounds());
        booleanMap.setMarked(2, 2);
        booleanMap.setMarked(3, 2);


        //check before running
        var shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0 },
                { 0, 0, 0, 0, 0 }
        };
        var flooded = booleanMap.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));


        //run flood shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        FloodPlaneComputer.RunUntilEscapeFoundOrPlaneDone(shader);

        var idx = shader.escapeIdx.ToArray();
        var escapePoints = Array.ConvertAll(shader.escapePoints.ToArray(), new Converter<int2, (int x, int y)>(i2 => (i2.X, i2.Y)));

        Assert.That(FloodPlaneComputer.GetFoundEscapePoints(shader).Length, Is.EqualTo(0));

        shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 0 }
        };
        flooded = FloodPlaneComputer.MarkedMapFromShader(shader).ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));

    }


    [Test]
    public void FloodsHoleMultiLevel()
    {
        DummyDimension heightMap = new DummyDimension((5, 4), 7);
        heightMap.FromGPUdata(new int[,] {
                { 7, 7, 7, 7, 7},
                { 7, 3, 3, 4, 7},
                { 3, 6, 5, 6, 7},
                { 7, 5, 7, 6, 1}, /* 5 is the escape point */
            });
        BooleanMap booleanMap = new BooleanMap(heightMap.Bounds());


        //check before running
        var shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
        };
        var flooded = booleanMap.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));


        //run flood shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        var (marked, escapes) = FloodPlaneComputer.ClimbHole(shader, (1,1), 10000, 10, 3);


        //verify results
        shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 0 },
                { 0, 0, 0, 1, 0 }
        };
        flooded = marked.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));
        CollectionAssert.AreEquivalent(
            escapes,
            new (int x, int y)[] { (0, 2), (1, 3) });
    }

    [Test]
    public void DoesntFlowBackIntoKnownHole()
    {
        DummyDimension heightMap = new DummyDimension((8, 5), 7);
        heightMap.FromGPUdata(new int[,] {
                { 7, 7, 7, 7, 7, 7, 7, 7},
                { 7, 2, 2, 5, 4, 7, 7, 7}, /*4s are known lake which we ignore */
                { 7, 2, 2, 5, 4, 5, 1, 7}, /*1 is exit point*/
                { 7, 7, 7, 7, 7, 7, 7, 7},
                { 7, 7, 7, 7, 7, 7, 7, 7},
        });
        BooleanMap booleanMap = new BooleanMap(heightMap.Bounds());
        //mark the lake we spawn from
        booleanMap.setMarked(4, 1);
        booleanMap.setMarked(4, 2);


        //check before running
        var shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },

        };
        var flooded = booleanMap.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));

        //run flood shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        var (marked, escapes) = FloodPlaneComputer.ClimbHole(shader, (2,1), 10000, 10, 0);

        //verify results
        shouldBeFlooded = new int[,]
               {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },

               };
        flooded = marked.ToGpuData();
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));
        CollectionAssert.AreEquivalent(
            escapes,
            new (int x, int y)[] { (6, 2) });
    }
}