using ComputeSharp;

[AutoConstructor]
public readonly partial struct ExpandDistanceShader : IComputeShader
{
    public readonly ReadWriteTexture2D<uint> distanceMap;
    public readonly ReadOnlyTexture2D<uint> heightMap;
    public readonly ReadWriteBuffer<int> changed;

    public static readonly uint STRAIGHT = 10;
    public static readonly uint DIAGONAL = 14;

    public static uint ignore = uint.MaxValue - 1000;
    public uint getHeight(int x, int y, int2 xy)
    {
        return heightMap[xy + new int2(x, y)];
    }

    public uint getDistance(int x, int y, int2 xy)
    {
        return distanceMap[xy + new int2(x, y)];
    }

    public uint distanceOrDefault(int x, int y, int2 XY, uint ownHeight)
    {
        if (XY.Y + y >= 0 &&
            XY.Y + y < distanceMap.Height &&
            XY.X + x >= 0 &&
            XY.X + x < distanceMap.Width &&
            getHeight(x, y, XY) == ownHeight &&
            getDistance(x, y, XY) != 0)
        {
            return getDistance(x, y, XY);
        }
        else
        {
            return ignore;
        }
    }

    public uint chooseSmaller(uint a, uint b)
    {
        return a < b ? a : b;
    }

    public void Execute()
    {
        int2 XY = ThreadIds.XY;
        uint ownHeight = getHeight(0, 0, XY);

        uint distance = distanceMap[ThreadIds.XY];

        //choose the neighbour with the lowest value and add 10
        distance = distance == 0 ? uint.MaxValue : distance;
        distance = chooseSmaller(distance, distanceOrDefault(0, -1, XY, ownHeight) + STRAIGHT);
        distance = chooseSmaller(distance, distanceOrDefault(0, 1, XY, ownHeight) + STRAIGHT);
        distance = chooseSmaller(distance, distanceOrDefault(1, 0, XY, ownHeight) + STRAIGHT);
        distance = chooseSmaller(distance, distanceOrDefault(-1, 0, XY, ownHeight) + STRAIGHT);

        distance = chooseSmaller(distance, distanceOrDefault(1, -1, XY, ownHeight) + DIAGONAL);
        distance = chooseSmaller(distance, distanceOrDefault(-1, -1, XY, ownHeight) + DIAGONAL);
        distance = chooseSmaller(distance, distanceOrDefault(-1, 1, XY, ownHeight) + DIAGONAL);
        distance = chooseSmaller(distance, distanceOrDefault(1, 1, XY, ownHeight) + DIAGONAL);

        //distance is not default value and has changed
        if (distance < ignore && distance != distanceMap[ThreadIds.XY])
        {
            changed[0] = 1;
            distanceMap[ThreadIds.XY] = distance;
        }
    }
}