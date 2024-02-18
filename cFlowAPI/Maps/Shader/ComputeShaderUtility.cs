using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using application.Maps.Shader;
using ComputeSharp;

namespace cFlowAPI.Maps.Shader;

internal class ComputeShaderUtility
{
    public static Bitmap SunlightFromHeightmap(Bitmap heightmap)
    {
        return RunShader(heightmap,
            (texture2D, writeTexture2D) => new SunlightMapShader(texture2D, writeTexture2D));
    }

    public static Bitmap RunShader<T>(Bitmap heightBitmap, Func<ReadOnlyTexture2D<uint>, ReadWriteTexture2D<uint>, T> createShader) where T : struct, IComputeShader
    {
        using ReadOnlyTexture2D<uint> input = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D(ToReadOnlySpan(heightBitmap), heightBitmap.Width, heightBitmap.Height);

        using ReadWriteTexture2D<uint> output = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(input.Width, input.Height);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // Launch the shader
        var shader = createShader(input, output);
        GraphicsDevice.GetDefault().For(input.Width, input.Height, shader);

        Debug.WriteLine($"running shader took {stopwatch.ElapsedMilliseconds / 1000f} seconds");
        stopwatch.Restart();

        uint[] outArr = new uint[input.Width * input.Height];

        // Get the data back
        output.CopyTo(outArr);
        stopwatch.Restart();
        Debug.WriteLine($"GPU->HEAD copy took {stopwatch.ElapsedMilliseconds / 1000f} seconds");

        Bitmap image = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
        CopyArrayToBitmap(outArr, image);
        stopwatch.Restart();
        Debug.WriteLine($"HEAP->Bitmap took {stopwatch.ElapsedMilliseconds / 1000f} seconds");
        return image;
    }

    static ReadOnlySpan<uint> ToReadOnlySpan(Bitmap bitmap)
    {
        // Lock the bitmap data
        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        var ptr = bmpData.Scan0;
        try
        {
            unsafe
            {
                // Calculate the total number of bytes in the bitmap
                int pixelCount = bmpData.Width * bmpData.Height;

                // Create a read-only span from the locked bitmap data
                ReadOnlySpan<uint> span = new ReadOnlySpan<uint>(bmpData.Scan0.ToPointer(), pixelCount);

                // Return the read-only span
                return span;
            }
        }
        finally
        {
            // Unlock the bitmap data
            bitmap.UnlockBits(bmpData);
        }
    }

    public static void CopyArrayToBitmap(uint[] sourceArray, Bitmap bitmap)
    {
        // Ensure the dimensions of the sourceArray match the dimensions of the bitmap
        if (sourceArray.Length != bitmap.Width * bitmap.Height)
        {
            throw new ArgumentException("Array dimensions do not match bitmap dimensions.");
        }

        Debug.Assert(bitmap.PixelFormat == PixelFormat.Format32bppArgb);

        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
            bitmap.PixelFormat);
        var ptr = bmpData.Scan0;

        try
        {
            unsafe
            {
                // Calculate the total number of bytes in the bitmap
                int pixelCount = bmpData.Width * bmpData.Height;
                Marshal.Copy((int[])(object)sourceArray, 0, destination: ptr, pixelCount);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("oh no :(");
        }
        finally
        {
            // Always unlock the bitmap

            bitmap.UnlockBits(bmpData);
        }
    }
}