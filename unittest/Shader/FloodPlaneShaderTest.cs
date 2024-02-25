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
}