using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using application.Maps;
using application.Maps.flowMap;
using cFlowApi.Heightmap;
using ComputeSharp;

namespace cFlowAPI.Maps.Shader
{

    public partial class FloodPlaneComputer
    {
        public static bool didChangeAndReset(Shader shader)
        {
            var didChange = shader.changed.ToArray()[0] == 1;
            shader.changed.CopyFrom(new int[1]);
            return didChange;
        }

        public static Shader FromMaps(DummyDimension heightMap, BooleanMap floodMap, int height)
        {
            var heightData = GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D<uint>(heightMap.ToGPUdata());
            var boolDataArray = floodMap.ToGpuData();
            var boolData = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(boolDataArray);
            var changedBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<int>(new int[1]);

            var shader = new FloodPlaneComputer.Shader(heightData, boolData, changedBuffer, (uint) height);
            return shader;
        }

        public static Shader WithHeightAndUnchanged(int height, Shader shader)
        {
            shader.changed.CopyFrom(new int[1]);
            return new FloodPlaneComputer.Shader(shader.heightTexture, shader.MarkedTexture, shader.changed, 0);
        }

        public static BooleanMap MarkedMapFromShader(Shader shader)
        {
            var booleanMap = new BooleanMap((shader.MarkedTexture.Width, shader.MarkedTexture.Height));
            booleanMap.FromGpuData(shader.MarkedTexture.ToArray());
            return booleanMap;
        }

        public static BooleanMap RunUntilEscapeFoundOrPlaneDone(Shader shader, int maxIterations = 10000)
        {
            for (int i = 0; i < maxIterations; i++)
            {
                GraphicsDevice.GetDefault().For(shader.heightTexture.Width, shader.heightTexture.Height, shader);
                var didChange = didChangeAndReset(shader);
                if (!didChange)
                    break;
            }
            var floodMap = new BooleanMap((shader.heightTexture.Width, shader.heightTexture.Height));
            floodMap.FromGpuData(shader.MarkedTexture.ToArray());
            return floodMap;
        }

        public static (int x, int y)[] GetFoundEscapePoints(Shader shader) {
            return [];
        }

        [AutoConstructor]
        public partial struct Shader : IComputeShader
        {
            public readonly ReadOnlyTexture2D<uint> heightTexture;
            public readonly ReadWriteTexture2D<int> MarkedTexture;
            public readonly ReadWriteBuffer<int> changed;
            public readonly uint currentHeight;

            public uint getHeight(int2 xy)
            {
                return heightTexture[xy];
            }

            public bool isMarked(int2 xy)
            {
                return MarkedTexture[xy] == 1;
            }

            public int2 neighbour(int x, int y, int2 self)
            {
                return self + new int2(x, y);
            }
            public bool FloodedEqualNeighbour(int2 xy)
            {
                return xy.X >= 0 &&
                       xy.Y >= 0 &&
                       xy.X < heightTexture.Width &&
                       xy.Y < heightTexture.Height &&
                       getHeight(xy) == currentHeight &&
                       isMarked(xy);
            }
            public void Execute()
            {
                int2 XY = ThreadIds.XY;
                uint ownHeight = heightTexture[XY];
                if (
                    MarkedTexture[XY] == 0 && 
                    ownHeight == currentHeight &&
                    (
                    FloodedEqualNeighbour(neighbour(-1, 0, XY)) ||
                    FloodedEqualNeighbour(neighbour(1, 0, XY)) ||
                    FloodedEqualNeighbour(neighbour(0, 1, XY)) ||
                    FloodedEqualNeighbour(neighbour(0, -1, XY))))
                {
                    MarkedTexture[XY] = 1;
                    changed[0] = 1;
                }
            }
        }
    }

}
