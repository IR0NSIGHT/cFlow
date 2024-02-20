using cFlowApi.Heightmap;
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
        //FIXME why is the data here Transposed?
        Assert.That(flowData, Is.EqualTo(new uint[,]
        {
            { NaturalEdgeShader.UNKNOWN,  NaturalEdgeShader.KNOWN | NaturalEdgeShader.DOWN, NaturalEdgeShader.UNKNOWN, },
            { NaturalEdgeShader.KNOWN | NaturalEdgeShader.LEFT, NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN,},
            { NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN, NaturalEdgeShader.UNKNOWN }
        }));
    }

    [Test]
    public void DimensionToGPUTexture()
    {
        var dim = new DummyDimension((5, 7), 17);
        dim.SetHeight((3,6),27);
        dim.SetHeight((1,2), 315);
        dim.SetHeight((4,6), 1);
        dim.SetHeight((0,0), 1);

        uint[,] arr = dim.ToGPUdata();
        foreach (var point in dim.iterator().Points())
        {
            Assert.That(dim.GetHeight(point) == arr[point.x, point.y]);
        }
    }
}