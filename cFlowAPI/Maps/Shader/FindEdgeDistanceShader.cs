using ComputeSharp;

namespace cFlowAPI.Maps.Shader;


[AutoConstructor]
public readonly partial struct FindEdgeDistanceShader : IComputeShader
{
    public readonly ReadOnlyTexture2D<uint> heightmap;
    public readonly ReadWriteTexture2D<uint> flowmap;

    public static readonly uint EDGE = 10;

    public void Execute()
    {
        uint flowValue = 0;
        int posX = ThreadIds.X;
        int posY = ThreadIds.Y;

        bool left = ThreadIds.Y > 0 &&
                    heightmap[ThreadIds.XY + new int2(0, -1)] < heightmap[ThreadIds.XY];

        bool right = ThreadIds.Y + 1 < heightmap.Height &&
                     heightmap[ThreadIds.XY + new int2(0, 1)] < heightmap[ThreadIds.XY];

        bool down = ThreadIds.X > 0 &&
                    heightmap[ThreadIds.XY + new int2(-1, 0)] < heightmap[ThreadIds.XY];

        bool up = ThreadIds.X + 1 < heightmap.Width &&
                  heightmap[ThreadIds.XY + new int2(1, 0)] < heightmap[ThreadIds.XY];

        if (left || right || up || down)
            flowValue = EDGE;

        //int to uint32 ARGB
        flowmap[ThreadIds.XY] = flowValue;
    }
}