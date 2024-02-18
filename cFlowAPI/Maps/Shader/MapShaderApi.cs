using System.Drawing;
using application.Maps.Shader;

namespace cFlowAPI.Maps.Shader;

public class MapShaderApi
{
    public static Bitmap SunlightFromHeightmap(Bitmap heightmap)
    {
        return ComputeShaderUtility.RunShader(heightmap,
            (texture2D, writeTexture2D) => new SunlightMapShader(texture2D, writeTexture2D));
    }

    public static Bitmap ContourFromHeightmap(Bitmap heightmap)
    {
        return ComputeShaderUtility.RunShader(heightmap,
            (texture2D, writeTexture2D) => new ContourMapShader(texture2D, writeTexture2D));
    }
}