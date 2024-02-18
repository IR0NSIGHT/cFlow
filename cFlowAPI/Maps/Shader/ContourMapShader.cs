using ComputeSharp;

namespace cFlowAPI.Maps.Shader;


[AutoConstructor]
internal readonly partial struct ContourMapShader : IComputeShader
{
    public readonly ReadOnlyTexture2D<uint> heightmap;
    public readonly ReadWriteTexture2D<uint> output;

    public void Execute()
    {
        uint color = 0x00;
        int posX = ThreadIds.X;
        int posY = ThreadIds.Y;
        if (posX > 0 && posX < heightmap.Width && posY > 0 && posY < heightmap.Height)
        {
            if (
                heightmap[ThreadIds.XY] % 10 == 0 && 
                (
                heightmap[ThreadIds.XY - new int2(-1, 0)] < heightmap[ThreadIds.XY] ||
                heightmap[ThreadIds.XY - new int2( 1, 0)] < heightmap[ThreadIds.XY] ||
                heightmap[ThreadIds.XY - new int2(0, -1)] < heightmap[ThreadIds.XY] ||
                heightmap[ThreadIds.XY - new int2( 0, 1)] < heightmap[ThreadIds.XY])
                )
            {
                //its on a border and one level divisible by 10
                color |= 0xFF000000;
            }


        }
        //int to uint32 ARGB
        output[ThreadIds.XY] = color;
    }
}