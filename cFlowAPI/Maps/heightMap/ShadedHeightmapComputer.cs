using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using ComputeSharp;
using TerraFX.Interop.Windows;

namespace application.Maps.heightMap;

public class ShadedHeightmapComputer
{
    public static Bitmap RunShader(Bitmap heightBitmap)
    {

        //    // Get a read-only span of the bitmap data
        //    ReadOnlySpan<byte> bitmapData = GetBitmapData(heightBitmap);
        //    GraphicsDevice.GetDefault().LoadReadOnlyTexture2D<R8, float>(bitmapData);

        // Allocate a GPU buffer and copy the data to it.
        // We want the shader to modify the items in-place, so we
        // can allocate a single read-write buffer to work on.
        using ReadOnlyTexture2D<R8, float> input =
            GraphicsDevice.GetDefault().LoadReadOnlyTexture2D<R8, float>("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\medium_flats_brokenUp.png");

        using ReadWriteTexture2D<R8, float> output = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<R8, float>(input.Width, input.Height);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        // Launch the shader
        GraphicsDevice.GetDefault().For(input.Width, input.Height, new MultiplyByTwo(input, output));

        Debug.WriteLine($"running shader took {stopwatch.ElapsedMilliseconds / 1000f} seconds");
        stopwatch.Restart();

        R8[] outArr = new R8[input.Width * input.Height];
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

    static ReadOnlySpan<byte> GetBitmapData(Bitmap bitmap)
    {
        // Lock the bitmap data
        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

        try
        {
            unsafe
            {
                // Calculate the total number of bytes in the bitmap
                int byteCount = bmpData.Stride * bmpData.Height;

                // Create a read-only span from the locked bitmap data
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(bmpData.Scan0.ToPointer(), byteCount);

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

    public static void CopyArrayToBitmap(R8[] array, Bitmap bmp)
    {
        // Ensure the dimensions of the array match the dimensions of the bitmap
        if (array.Length != bmp.Width * bmp.Height)
        {
            throw new ArgumentException("Array dimensions do not match bitmap dimensions.");
        }

        // Get the number of bytes per pixel
        int bytesPerPixel = 4;

        // Copy the array values into the bitmap

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                int index = y * bmp.Width / bytesPerPixel + x;
                byte value = array[index].R;
                
                bmp.SetPixel(x,y,Color.FromArgb(255,value,value,value));//FIXME index * 2 is correct
            }
        }
    }

    public static void CopyBitmapToArray(int[,] array, Bitmap bmp)
    {
        // Ensure the dimensions of the array match the dimensions of the bitmap
        if (array.GetLength(0) != bmp.Width || array.GetLength(1) != bmp.Height)
        {
            throw new ArgumentException("Array dimensions do not match bitmap dimensions.");
        }

        // Lock the bitmap data for direct access
        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.WriteOnly,
            bmp.PixelFormat);

        // Get the number of bytes per pixel
        int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;

        // Copy the array values into the bitmap
        unsafe
        {
            byte* ptr = (byte*)bmpData.Scan0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    // Calculate the index in the 1D array
                    int index = y * bmpData.Stride + x * bytesPerPixel;

                    // Set the pixel value from the array
                    int value = array[x, y]; // Assuming x corresponds to width and y corresponds to height
                    ptr[index] = (byte)value; // Set the pixel value (assuming 1 byte per pixel)
                }
            }
        }

        // Unlock the bitmap data
        bmp.UnlockBits(bmpData);
    }
}


[AutoConstructor]
public readonly partial struct MultiplyByTwo : IComputeShader
{
    public readonly ReadOnlyTexture2D<R8, float> input;
    public readonly ReadWriteTexture2D<R8, float> output;

    public void Execute()
    {
        int ownSunshine = 127;
        int posX = ThreadIds.X;
        int posY = ThreadIds.Y;
        if (posX > 0 && posX < input.Width && posY > 0 && posY < input.Height)
        {
            //block left of me is higher => reduce sunshine
            if (input[ThreadIds.XY - new int2(-1, 0)] > input[ThreadIds.XY])
                ownSunshine -= 32;
            //im higher than block right of me => increase sunshine
            if (input[ThreadIds.XY] < input[ThreadIds.XY - new int2(1, 0)])
                ownSunshine += 32;

            //block bottom of me is higher => reduce sunshine
            if (input[ThreadIds.XY - new int2(0, -1)] > input[ThreadIds.XY])
                ownSunshine -= 32;
            //im higher than block top of me => increase sunshine
            if (input[ThreadIds.XY] < input[ThreadIds.XY - new int2(0, 1)])
                ownSunshine += 32;
        }
        output[ThreadIds.XY] = ownSunshine;
        //DEBUG
        output[ThreadIds.XY] = input[ThreadIds.XY];

    }
}