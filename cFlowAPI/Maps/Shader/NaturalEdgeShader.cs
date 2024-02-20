using cFlowApi.Heightmap;
using ComputeSharp;

namespace cFlowAPI.Maps.Shader;

[AutoConstructor]
public readonly partial struct NaturalEdgeShader: IComputeShader
{
    public readonly ReadOnlyTexture2D<uint> heightmap;
    public readonly ReadWriteTexture2D<uint> flowmap;

    public static readonly uint KNOWN = 0b10000000;
    public static readonly uint UNKNOWN = 0b00000000;
    public static readonly uint RIGHT = 0b1000;
    public static readonly uint LEFT = 0b0100;
    public static readonly uint UP = 0b0010;
    public static readonly uint DOWN = 0b0001;

    public void Execute()
    {
        uint flowValue = flowmap[ThreadIds.XY];
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

            if (left)
                flowValue |= LEFT;
            if (right)
                flowValue |= RIGHT;
            if (up)
                flowValue |= UP;
            if (down)
                flowValue |= DOWN;
            if (left || right || up || down)
                flowValue |= KNOWN;
        
        //int to uint32 ARGB
        flowmap[ThreadIds.XY] = flowValue;
    }
}