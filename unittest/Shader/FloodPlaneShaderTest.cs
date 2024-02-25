using application.Maps;
using cFlowApi.Heightmap;
using cFlowAPI.Maps.Shader;
using ComputeSharp;

namespace unittest.Shader;

[TestFixture]
public class FloodPlaneShaderTest
{
    [Test]
    public void FloodOneLevelSimpleStepByStep()
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


        //run flood shader
        var shader = FloodPlaneComputer.FromMaps(heightMap, booleanMap, 3);
        GraphicsDevice.GetDefault().For(heightMap.Bounds().x, heightMap.Bounds().y, shader);

        //verify first iteration
        flooded = FloodPlaneComputer.MarkedMapFromShader(shader).ToGpuData();
        shouldBeFlooded = new int[,]
        {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 1, 1, 0, 0 },
                { 0, 1, 0, 0, 0 }
        };
        Assert.That(flooded, Is.EqualTo(shouldBeFlooded));
        Assert.That(FloodPlaneComputer.didChangeAndReset(shader));


        //run second time
        GraphicsDevice.GetDefault().For(heightMap.Bounds().x, heightMap.Bounds().y, shader);

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
        Assert.That(FloodPlaneComputer.didChangeAndReset(shader));

        //run third time => nothing changed
        GraphicsDevice.GetDefault().For(heightMap.Bounds().x, heightMap.Bounds().y, shader);

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
        Assert.That(FloodPlaneComputer.didChangeAndReset(shader), Is.False);
    }

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
    public void CanFindEscapePoint()
    {
        DummyDimension heightMap = new DummyDimension((5, 4), 7);
        heightMap.FromGPUdata(new int[,] {
                { 1, 7, 7, 1, 7},
                { 7, 3, 3, 7, 3},
                { 7, 3, 3, 7, 7},
                { 7, 3, 3, 1, 7} /*<= 1 is escape point */
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

        //TODO 

    }
}