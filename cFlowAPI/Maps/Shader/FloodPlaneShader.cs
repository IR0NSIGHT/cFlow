using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static int getAmountChangedAndReset(Shader shader)
        {
            var didChange = shader.changed.ToArray()[0];
            shader.changed.CopyFrom(new int[1]);
            return didChange;
        }

        public static Shader FromMaps(HeightMap heightMap, BooleanMap floodMap, int height)
        {
            var changedBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<int>(new int[1]);

            var escapePointIdxBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(new int[1]);
            var escapePointBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(new int2[100]);
            var shader = new FloodPlaneComputer.Shader(
                heightTexture: GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D<uint>(heightMap.ToGPUdata()),
                MarkedTexture: GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(floodMap.Bounds().x, floodMap.Bounds().y),
                didChangeMarkingTexture: GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(floodMap.Bounds().x, floodMap.Bounds().y),
                ignoredEscapes: GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D<int>(floodMap.ToGpuData()),
                changed: changedBuffer,
                currentHeight: GraphicsDevice.GetDefault().AllocateReadOnlyBuffer(new int[] { height }),
                escapePointIdxBuffer,
                escapePointBuffer);
            return shader;
        }

        public static BooleanMap MarkedMapFromShader(Shader shader)
        {
            var booleanMap = new BooleanMap((shader.MarkedTexture.Width, shader.MarkedTexture.Height));
            booleanMap.FromGpuData(shader.MarkedTexture.ToArray());
            return booleanMap;
        }

        public static (BooleanMap marked, (int x, int y)[] escapes) ClimbHole(Shader floodShader, (int x, int y) source, int maxLakeSurface = 10000, int maxDepth = 255, int startDepth = 0)
        {
            Debug.WriteLine($"climb hole from {source}");
            //prime beforeMarkedTexture with the start positions
            var startMap = new BooleanMap((floodShader.heightTexture.Width, floodShader.heightTexture.Height));
            startMap.setMarked(source.x, source.y);
            floodShader.MarkedTexture.CopyFrom(startMap.ToGpuData());
            var totalChanged = 0;
            //Run shader
            for (int i = startDepth; i < maxDepth; i++)
            {
                floodShader.currentHeight.CopyFrom(new int[] { i });

                var changed = RunUntilEscapeFoundOrPlaneDone(floodShader, maxLakeSurface - totalChanged);
                totalChanged += changed;

                if (GetFoundEscapePoints(floodShader).Length != 0)
                {
                    //TODO: remove currentheight marked points => this plane was not fully flooded.
                    var cleaner = new CleanUpShader(
                        floodShader.heightTexture,
                        floodShader.MarkedTexture,
                        floodShader.ignoredEscapes,
                        floodShader.currentHeight
                        );
                    GraphicsDevice.GetDefault().For(floodShader.heightTexture.Width, floodShader.heightTexture.Height, cleaner);
                    return (MarkedMapFromShader(floodShader), GetFoundEscapePoints(floodShader));
                }

                if (maxLakeSurface < totalChanged)
                {
                    Debug.WriteLine("lake exceeded maximum surface. abort now");
                    break;
                }
            }
            return (MarkedMapFromShader(floodShader), []);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="maxNewSurface"></param>
        /// <returns>marked map and total points changed</returns>
        private static int RunUntilEscapeFoundOrPlaneDone(Shader shader, int maxNewSurface)
        {
            var currentShader = shader;
            var totalChanged = 0;
            for (int i = 0; i < 100000/*safety margin*/; i++)
            {
                GraphicsDevice.GetDefault().For(shader.heightTexture.Width, shader.heightTexture.Height, currentShader);
                var didChange = getAmountChangedAndReset(currentShader);
                totalChanged += didChange;
                if (totalChanged > maxNewSurface)
                    return totalChanged;
                if (didChange == 0)
                    return totalChanged;
                if (shader.escapeIdx.ToArray()[0] != 0)
                    return totalChanged;
            }

            throw new Exception("safety margin stopped infinte loop for floodplane shader");
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
            public readonly ReadWriteTexture2D<int> MarkedTexture;
            public readonly ReadWriteTexture2D<int> didChangeMarkingTexture;

            public readonly ReadOnlyTexture2D<int> ignoredEscapes;


            public readonly ReadWriteBuffer<int> changed;
            public readonly ReadOnlyBuffer<int> currentHeight;
            public readonly ReadWriteBuffer<int> escapeIdx;
            public readonly ReadWriteBuffer<int2> escapePoints;

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
            public bool FloodedNeighbourAtCurrentHeight(int2 xy)
            {
                return xy.X >= 0 &&
                       xy.Y >= 0 &&
                       xy.X < heightTexture.Width &&
                       xy.Y < heightTexture.Height &&
                       didChangeMarkingTexture[xy] == 0 &&  //neighbour xy wasnt changed in this shader run.
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
                didChangeMarkingTexture[XY] = 0;

                if (
                    MarkedTexture[XY] == 0 &&
                    (
                    FloodedNeighbourAtCurrentHeight(neighbour(-1, 0, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(1, 0, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(0, 1, XY)) ||
                    FloodedNeighbourAtCurrentHeight(neighbour(0, -1, XY))))
                {
                    if (ownHeight == currentHeight[0] || ignoredEscapes[XY] == 1)
                    {
                        MarkedTexture[XY] = 1;
                        didChangeMarkingTexture[XY] = 1;
                        Hlsl.InterlockedAdd(ref changed[0], 1);
                    }
                    else if (ownHeight < currentHeight[0] && ignoredEscapes[XY] != 1)
                    {
                        int idx = 0;
                        Hlsl.InterlockedAdd(ref escapeIdx[0], 1, out idx);
                        escapePoints[idx] = ThreadIds.XY;
                    }

                }

            }
        }

        /// <summary>
        /// will unmark all points that are not ignored and at currentHeight.
        /// </summary>
        [AutoConstructor]
        public partial struct CleanUpShader : IComputeShader
        {
            public readonly ReadOnlyTexture2D<uint> heightTexture;
            public readonly ReadWriteTexture2D<int> MarkedTexture;

            public readonly ReadOnlyTexture2D<int> ignoredEscapes;

            public readonly ReadOnlyBuffer<int> currentHeight;


            public void Execute()
            {
                int2 XY = ThreadIds.XY;
                uint ownHeight = heightTexture[XY];
                if (ownHeight == currentHeight[0] && ignoredEscapes[XY] == 0)
                {
                    MarkedTexture[XY] = 0;
                }
            }
        }
    }

}
