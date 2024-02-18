using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ComputeSharp;
using TerraFX.Interop.Windows;

namespace application.Maps.heightMap;

public class ShadedHeightmapComputer
{
    public static Bitmap RunShader(Bitmap heightBitmap)
    {

        //    // Get a read-only span of the bitmap data
        //    ReadOnlySpan<byte> bitmapData = ToReadOnlySpan(heightBitmap);
        //    GraphicsDevice.GetDefault().LoadReadOnlyTexture2D<Rgba32, float4>(bitmapData);

        // Allocate a GPU buffer and copy the data to it.
        // We want the shader to modify the items in-place, so we
        // can allocate a single read-write buffer to work on.

        using ReadOnlyTexture2D<uint> input = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D<uint>(ToReadOnlySpan(heightBitmap), heightBitmap.Width, heightBitmap.Height);

        using ReadWriteTexture2D<uint> output = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(input.Width, input.Height);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // Launch the shader
        GraphicsDevice.GetDefault().For(input.Width, input.Height, new MultiplyByTwo(input, output));

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


[AutoConstructor]
public readonly partial struct MultiplyByTwo : IComputeShader
{
    public readonly ReadOnlyTexture2D<uint> input;
    public readonly ReadWriteTexture2D<uint> output;

    public void Execute()
    {
        uint ownSunshine = 127;
        int posX = ThreadIds.X;
        int posY = ThreadIds.Y;
        if (posX > 0 && posX < input.Width && posY > 0 && posY < input.Height)
        {
            //block left of me is higher => reduce sunshine
            if (input[ThreadIds.XY - new int2(-1, 0)] > input[ThreadIds.XY])
                ownSunshine += 32;
            //block lef of me is lower => increase sunshine
            if (input[ThreadIds.XY - new int2(-1, 0)] < input[ThreadIds.XY])
                ownSunshine -= 32;

            //block left of me is higher => reduce sunshine
            if (input[ThreadIds.XY - new int2(1, 0)] > input[ThreadIds.XY])
                ownSunshine -= 32;
            //block lef of me is lower => increase sunshine
            if (input[ThreadIds.XY - new int2(1, 0)] < input[ThreadIds.XY])
                ownSunshine += 32;

       //     //block bottom of me is higher => reduce sunshine
       //     if (input[ThreadIds.XY - new int2(0, -1)] > input[ThreadIds.XY])
       //         ownSunshine -= 32;
       //     //im higher than block top of me => increase sunshine
       //     if (input[ThreadIds.XY - new int2(0, -1)] < input[ThreadIds.XY])
       //         ownSunshine += 32;
        }
        //int to uint32 ARGB
        output[ThreadIds.XY] = 0xFF000000 | ownSunshine << 16 | ownSunshine << 8 | ownSunshine;
        if (posX == posY)
        {
            output[ThreadIds.XY] = 0xFFFFFFFF;
        }

    }
}