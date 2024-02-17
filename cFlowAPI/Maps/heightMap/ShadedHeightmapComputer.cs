using ComputeSharp;

namespace application.Maps.heightMap;

public class ShadedHeightmapComputer
{
    public static void RunShader()
    {
        // Get some sample data
        int[] array = Enumerable.Range(1, 100).ToArray();

        // Allocate a GPU buffer and copy the data to it.
        // We want the shader to modify the items in-place, so we
        // can allocate a single read-write buffer to work on.
        using ReadWriteBuffer<int> buffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(array);

        // Launch the shader
        GraphicsDevice.GetDefault().For(buffer.Length, new MultiplyByTwo(buffer));

        // Get the data back
        buffer.CopyTo(array);
    }
}

[AutoConstructor]
public readonly partial struct MultiplyByTwo : IComputeShader
{
    public readonly ReadWriteBuffer<int> buffer;

    public void Execute()
    {
        buffer[ThreadIds.X] *= 2;
    }
}