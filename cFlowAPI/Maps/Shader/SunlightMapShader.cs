using ComputeSharp;

namespace application.Maps.Shader;

[AutoConstructor]
internal readonly partial struct SunlightMapShader : IComputeShader
{
    public readonly ReadOnlyTexture2D<uint> heightmap;
    public readonly ReadWriteTexture2D<uint> output;

    public void Execute()
    {
        uint ownSunshine = 127;
        int posX = ThreadIds.X;
        int posY = ThreadIds.Y;
        uint delta = 31;
        if (posX > 0 && posX < heightmap.Width && posY > 0 && posY < heightmap.Height)
        {
            if (heightmap[ThreadIds.XY - new int2(-1, 0)] > heightmap[ThreadIds.XY])
                ownSunshine += delta;
            if (heightmap[ThreadIds.XY - new int2(-1, 0)] < heightmap[ThreadIds.XY])
                ownSunshine -= delta;

            if (heightmap[ThreadIds.XY - new int2(1, 0)] > heightmap[ThreadIds.XY])
                ownSunshine -= delta;
            if (heightmap[ThreadIds.XY - new int2(1, 0)] < heightmap[ThreadIds.XY])
                ownSunshine += delta;

            if (heightmap[ThreadIds.XY - new int2(0, -1)] > heightmap[ThreadIds.XY])
                ownSunshine += delta;
            if (heightmap[ThreadIds.XY - new int2(0, -1)] < heightmap[ThreadIds.XY])
                ownSunshine -= delta;

            if (heightmap[ThreadIds.XY - new int2(0, 1)] > heightmap[ThreadIds.XY])
                ownSunshine -= delta;
            if (heightmap[ThreadIds.XY - new int2(0, 1)] < heightmap[ThreadIds.XY])
                ownSunshine += delta;

        }
        //int to uint32 ARGB
        output[ThreadIds.XY] = 0xFF000000 | ownSunshine << 16 | ownSunshine << 8 | ownSunshine;
    }
}