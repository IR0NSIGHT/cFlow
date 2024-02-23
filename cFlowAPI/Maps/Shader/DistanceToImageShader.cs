using ComputeSharp;

namespace cFlowAPI.Maps.Shader
{
    [AutoConstructor]
    public partial struct DistanceToImageShader: IComputeShader
    {
        public readonly ReadOnlyTexture2D<uint> distanceMap;
        public readonly ReadWriteTexture2D<Rgba32, Float4> argb32Image;
        public void Execute()
        {
            var isUnknow = distanceMap[ThreadIds.XY] == 0;

            if (isUnknow)
            {
                argb32Image[ThreadIds.XY] = new Float4(1, 1, 1, 1f);
            }
            else
            {
                argb32Image[ThreadIds.XY] = new Float4(0f, 1f - ((distanceMap[ThreadIds.XY] % 255) / 255f), 0f, 1f /*Alpha*/);
            }
        }
    }
}
