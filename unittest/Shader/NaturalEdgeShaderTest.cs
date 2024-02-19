using cFlowAPI.Maps.Shader;
using ComputeSharp;

namespace unittest.Shader;
[TestFixture]
public class NaturalEdgeShaderTest
{
    [Test]
    public void basic()
    {
        uint[,] heightData = new uint[,]
        {
            { 5, 10, 10, },
            { 10, 10, 10,},
            { 10, 10, 10 }
        };

        uint[,] flowData = new uint[,]
        {
            {NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN},
            {NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN},
            {NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN,NaturalEdgeShader.UNKNOWN}
        };

        using ReadOnlyTexture2D<uint> heightmap = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D<uint>(heightData);
        using ReadWriteTexture2D<uint> flowmap = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(flowData);
        var shader = new NaturalEdgeShader(heightmap, flowmap);
        GraphicsDevice.GetDefault().For(heightmap.Width, heightmap.Height, shader);


        flowmap.CopyTo(flowData);
        Assert.That(flowData, Is.EqualTo(new uint[,]
        {
            { NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.LEFT, NaturalEdgeShader.UNKNOWN, },
            { NaturalEdgeShader.DOWN, NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN,},
            { NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN }
        }));
    }
}