using ComputeSharp;

namespace application.Maps.heightMap;

public class ShadedHeightmapComputer
{
    public static void RunShader()
    {
        // Get some sample data
        int[] array = new int[]{0,0,0,0,0,1,2,3,4,4,4,4,3,2,1,0,0,0};

        // Allocate a GPU buffer and copy the data to it.
        // We want the shader to modify the items in-place, so we
        // can allocate a single read-write buffer to work on.
        using ReadOnlyBuffer<int> input = GraphicsDevice.GetDefault().AllocateReadOnlyBuffer(array);
        using ReadWriteBuffer<int> output = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(array);

        // Launch the shader
        GraphicsDevice.GetDefault().For(input.Length, new MultiplyByTwo(input, output));

        // Get the data back
        output.CopyTo(array);
    }
}

[AutoConstructor]
public readonly partial struct MultiplyByTwo : IComputeShader
{
    public readonly ReadOnlyBuffer<int> input;
    public readonly ReadWriteBuffer<int> output;

    public void Execute()
    {
        int ownSunshine = 127;
        if (ThreadIds.X > 0 && ThreadIds.X < input.Length - 1)
        {

            //block left of me is higher => reduce sunshine
            if (input[ThreadIds.X - 1] > input[ThreadIds.X])
                ownSunshine -= 32;
            //im higher than block right of me => increase sunshine
            if (input[ThreadIds.X] < input[ThreadIds.X + 1])
                ownSunshine += 32;
        }
        output[ThreadIds.X] = ownSunshine;

    }
}