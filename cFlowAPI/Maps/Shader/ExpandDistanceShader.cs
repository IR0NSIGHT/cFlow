using ComputeSharp;

[AutoConstructor]
public readonly partial struct ExpandDistanceShader : IComputeShader
{
    public readonly ReadWriteTexture2D<uint> distanceMap;
    public readonly ReadOnlyTexture2D<uint> heightMap;
    public readonly ReadWriteBuffer<bool> changed;

    public static readonly uint EDGE = 10;

    public void Execute()
    {
        uint ownHeight = heightMap[ThreadIds.XY];
        //threadId.X == x value == column in image
        //threadId.Y 
        uint distance = distanceMap[ThreadIds.XY];
        uint ignore = 0;
        uint down = ThreadIds.Y > 0 && heightMap[ThreadIds.XY + new int2(0, -1)] == ownHeight ?
            distanceMap[ThreadIds.XY + new int2(0, -1)] : ignore;

        uint up = ThreadIds.Y + 1 < distanceMap.Height && heightMap[ThreadIds.XY + new int2(0, 1)] == ownHeight ?
            distanceMap[ThreadIds.XY + new int2(0, 1)] : ignore;

        uint left = ThreadIds.X > 0 && heightMap[ThreadIds.XY + new int2(-1, 0)] == ownHeight ?
            distanceMap[ThreadIds.XY + new int2(-1, 0)] : ignore;


        uint right = ThreadIds.X + 1 < distanceMap.Width && heightMap[ThreadIds.XY + new int2(1, 0)] == ownHeight
            ? distanceMap[ThreadIds.XY + new int2(1, 0)] : ignore;

        //zero means untouched => choose greatest value to choose a neighbour guaranteed
        uint baseValue = distance == 0 ? uint.MaxValue : distance;

        //choose the neighbour with the lowest value and add 10
        baseValue = left != ignore && left < baseValue ? left + 10 : baseValue;
        baseValue = right != ignore && right < baseValue ? right + 10 : baseValue;
        baseValue = down != ignore && down < baseValue ? down + 10 : baseValue;
        baseValue = up != ignore && up < baseValue ? up + 10 : baseValue;

        if (baseValue != uint.MaxValue && distance != baseValue)
        {
            changed[0] = true;
            distanceMap[ThreadIds.XY] = baseValue;//(uint)((ThreadIds.Y << 8 & 0xFF00) | (ThreadIds.X & 0x00FF));
        }
    }
}