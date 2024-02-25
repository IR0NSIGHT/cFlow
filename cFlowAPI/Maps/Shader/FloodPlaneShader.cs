using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using application.Maps;
using application.Maps.flowMap;
using cFlowApi.Heightmap;
using ComputeSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var changedBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<int>(new int[1]);

            var escapePointIdxBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(new int[1]);
            var escapePointBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(new int2[100]);
            var shader = new FloodPlaneComputer.Shader(
                GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D<uint>(heightMap.ToGPUdata()),
                GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(floodMap.ToGpuData()),
                GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(floodMap.Bounds().x, floodMap.Bounds().y),
                changedBuffer, (uint)height, escapePointIdxBuffer, escapePointBuffer);
            return shader;
        }

        public static BooleanMap MarkedMapFromShader(Shader shader)
        {
            var booleanMap = new BooleanMap((shader.AfterMarkedTexture.Width, shader.AfterMarkedTexture.Height));
            booleanMap.FromGpuData(shader.AfterMarkedTexture.ToArray());
            return booleanMap;
        }

        public static (BooleanMap marked, (int x, int y)[] escapes) ClimbHole(Shader currentShader, int maxIterations = 10000, int maxDepth = 255, int startDepth = 0)
        {
            currentShader = new Shader(
            currentShader.heightTexture,
            currentShader.BeforeMarkedTexture,
            currentShader.AfterMarkedTexture,
            currentShader.changed,
            (uint)startDepth,
            currentShader.escapeIdx,
            currentShader.escapePoints
            );
            for (int i = startDepth; i < maxDepth; i++)
            {
                RunUntilEscapeFoundOrPlaneDone(currentShader, maxIterations);
                if (GetFoundEscapePoints(currentShader).Length != 0)
                {
                    return (MarkedMapFromShader(currentShader), GetFoundEscapePoints(currentShader));
                }
                currentShader = new Shader(
                currentShader.heightTexture,
                currentShader.AfterMarkedTexture,
                currentShader.BeforeMarkedTexture,
                currentShader.changed,
                (uint)i,
                currentShader.escapeIdx,
                currentShader.escapePoints
                );
            }
            return (MarkedMapFromShader(currentShader), []);
        }

        public static BooleanMap RunUntilEscapeFoundOrPlaneDone(Shader shader, int maxIterations = 10000)
        {
            var currentShader = shader;

            for (int i = 0; i < maxIterations; i++)
            {
                GraphicsDevice.GetDefault().For(shader.heightTexture.Width, shader.heightTexture.Height, currentShader);
                var didChange = didChangeAndReset(currentShader);
                if (!didChange)
                    break;
                if (shader.escapeIdx.ToArray()[0] != 0)
                    break;

                //apply output floodmap to new input
                currentShader = new Shader(
                    currentShader.heightTexture,
                    currentShader.AfterMarkedTexture,
                    currentShader.BeforeMarkedTexture,
                    currentShader.changed,
                   currentShader.currentHeight,
                   currentShader.escapeIdx,
                   currentShader.escapePoints
                    );
            }

            return MarkedMapFromShader(currentShader);
        }

        public static (int x, int y)[] GetFoundEscapePoints(Shader shader)
        {
            var rawPoints = shader.escapePoints.ToArray();
            Array.Resize(ref rawPoints, shader.escapeIdx.ToArray()[0]);
            var escapePoints = Array.ConvertAll(rawPoints, new Converter<int2, (int x, int y)>(i2 => (i2.X, i2.Y)));
            return escapePoints;
        }

        [AutoConstructor]
        public partial struct Shader : IComputeShader
        {
            public readonly ReadOnlyTexture2D<uint> heightTexture;
            public readonly ReadWriteTexture2D<int> BeforeMarkedTexture;
            public readonly ReadWriteTexture2D<int> AfterMarkedTexture;


            public readonly ReadWriteBuffer<int> changed;
            public readonly uint currentHeight;
            public readonly ReadWriteBuffer<int> escapeIdx;
            public readonly ReadWriteBuffer<int2> escapePoints;

            public uint getHeight(int2 xy)
            {
                return heightTexture[xy];
            }

            public bool isMarked(int2 xy)
            {
                return BeforeMarkedTexture[xy] == 1;
            }

            public int2 neighbour(int x, int y, int2 self)
            {
                return self + new int2(x, y);
            }
            public bool FloodedNeighbourAtCurrentHeight(int2 xy)
            {
                return xy.X >= 0 &&
                       xy.Y >= 0 &&
                       xy.X < heightTexture.Width &&
                       xy.Y < heightTexture.Height &&
            //TODO do i need this           getHeight(xy) == currentHeight &&
                       isMarked(xy);
            }
            public void Execute()
            {
                if (ThreadIds.X == 0 && ThreadIds.Y == 0)   //reset on each run
                {
                    escapeIdx[0] = 0;
                    changed[0] = 0;
                }


                int2 XY = ThreadIds.XY;
                uint ownHeight = heightTexture[XY];

                //copy over existing values
                AfterMarkedTexture[XY] = BeforeMarkedTexture[XY];

                if (
                    BeforeMarkedTexture[XY] == 0 &&
                    (
                    FloodedNeighbourAtCurrentHeight(neighbour(-1, 0, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(1, 0, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(0, 1, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(0, -1, XY))))
                {
                    if (ownHeight == currentHeight)
                    {
                        AfterMarkedTexture[XY] = 1;
                        changed[0] = 1;
                    }
                    else if (ownHeight < currentHeight)
                    {
                        //TBD
                        int idx = 0;
                        Hlsl.InterlockedAdd(ref escapeIdx[0], 1, out idx);
                        escapePoints[idx] = ThreadIds.XY;
                    }

                }

            }
        }
    }

}
