﻿
using System.Diagnostics;
using application.Maps;
using src.Maps.riverMap;
using static Point;

namespace cFlowAPI.Maps.riverMap
{
    public class FloodTool
    {
        private readonly IHeightMap _heightMap;
        public FloodTool(IHeightMap heightMap)
        {
            _heightMap = heightMap;
        }

        public struct FloodPlane
        {
            public List<(int x, int y)> borderRing;
            public BooleanMap planeMap;
            public List<(int x, int y)> escapePoints;
            public bool exceededMaxSurface;
        }

        public List<(int x, int y)> FloodArea((int x, int y) start, RiverMap targetRiverMap, int maxDepth = 10, int maxSurfaceBeforeExceeded = 100000)
        {
            int startZ = _heightMap.GetHeight(start);

            var lakeMap = new BooleanMap(targetRiverMap.Bounds());
            bool IsIgnoredPoint((int x, int y) p)
            {
                var marked = lakeMap.isMarked(p.x, p.y);
                return marked;
            };

            List<(int x, int y)> currentOuterMost = [start];
            for (int i = 0; i < maxDepth; i++)
            {
                var maxZ = startZ + i;
                currentOuterMost.Sort(); //TODO remove, is debug helper



                //mark outermost ring on lakemap. planecollection will ignore this ring when finding neighbours
                foreach (var point in currentOuterMost)
                {
                    lakeMap.setMarked(point.x, point.y);
                    Debug.Assert(lakeMap.isMarked(point.x, point.y));
                }
                lakeMap.ToImage().Save($"C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\debug\\lakemap_{i}_maxZ_{maxZ}.png");

                var floodPlane =
                    collectPlaneAtOrBelow(
                        currentOuterMost,
                        maxZ, IsIgnoredPoint
                        ,
                        maxSurfaceBeforeExceeded,
                        true);

                if (floodPlane.exceededMaxSurface)
                    return new List<(int x, int y)>();

                currentOuterMost = floodPlane.borderRing;
                foreach (var p in floodPlane.planeMap.IterateMarked())
                {
                    targetRiverMap.SetAsRiver(p.x, p.y);
                    lakeMap.setMarked(p.x, p.y);
                }

                if (floodPlane.escapePoints.Count != 0)
                {
                    floodPlane.escapePoints.ForEach(p =>
                        Debug.Assert(!lakeMap.isMarked(p.x, p.y)));
                    return floodPlane.escapePoints;
                }
            }

            return new List<(int x, int y)>();

        }

        /// <summary>
        /// test if this point has a neighbour that is aboveZ
        /// => point is touching the border of the plane defined by belowZ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isAboveZ"></param>
        /// <returns></returns>
        private bool isBorderPoint((int x, int y) pos, Func<(int x, int y), bool> isAboveZ)
        {
            var neighs = new (int x, int y)[] { Up(pos), Down(pos), Left(pos), Right(pos) };
            foreach (var neigh in neighs)
            {
                if (_heightMap.inBounds(neigh.x, neigh.y) && isAboveZ(neigh))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// find largest connected area of blocks that originate from startingPositions and are all below level of belowZ
        /// </summary>
        /// <param name="startingPositions"></param>
        /// <param name="maxZ"></param>
        /// <param name="maxSurfaceBeforeExceeded">lenght of border ring before aborting search, disable with -1 (default)</param>
        /// <returns></returns>
        public FloodPlane collectPlaneAtOrBelow(List<(int x, int y)> startingPositions, int maxZ, Func<(int x, int y), bool> isIgnoredPoint, int maxSurfaceBeforeExceeded = -1, bool abortOnLowerFound = false)
        {
            BooleanMap toBecomeRiver = new BooleanMap(_heightMap.Bounds());
            PointCategory pointCategory = new PointCategory()
            {
                isBelowZ = pos =>
                    _heightMap.GetHeight(pos) < maxZ,
                isEqualZ = pos =>
                    _heightMap.GetHeight(pos) == maxZ,
                isAboveZ = pos =>
                    _heightMap.GetHeight(pos) > maxZ,
                isIgnoredPoint = isIgnoredPoint
            };

            List<(int x, int y)> outerMostRing = []; 

            var nextPositions = startingPositions;

            startingPositions.ForEach(p => toBecomeRiver.setMarked(p.x,p.y));
            startingPositions
                .Where(p => isBorderPoint(p, pointCategory.isAboveZ))
                .ToList().ForEach(outerMostRing.Add);

            while (true)
            {
                var (nextRing, didFindLower) = GetTouchingUnseen(nextPositions, pointCategory, toBecomeRiver, abortOnLowerFound);
                //ring has found everything, nothing more to do
                if (nextRing.Count == 0)
                    break;

                if (didFindLower && abortOnLowerFound)   //we found an escape from this plane => return all points that were flooded and the escape points.
                {
                    foreach (var tuple in nextRing)
                    {
                        Debug.Assert(!toBecomeRiver.isMarked(tuple.x, tuple.y));
                        //TODO    Debug.Assert(_heightMap.GetHeight(tuple) < 139);
                    }

                    return new FloodPlane()
                    {
                        borderRing = [],
                        escapePoints = nextRing,
                        exceededMaxSurface = false,
                        planeMap = toBecomeRiver
                    };
                }

                nextPositions = nextRing;

                foreach (var pos in nextRing)
                {
                    if (isBorderPoint(pos, pointCategory.isAboveZ))
                        outerMostRing.Add(pos);
                }

                //ring has exceeded maximum size
                if (maxSurfaceBeforeExceeded != -1 && toBecomeRiver.getMarkedAmount() > maxSurfaceBeforeExceeded)
                {
                    break;
                }
            }
            var floodPlane = new FloodPlane()
            {
                borderRing = outerMostRing,
                escapePoints = [],
                exceededMaxSurface = maxSurfaceBeforeExceeded != -1 && toBecomeRiver.getMarkedAmount() > maxSurfaceBeforeExceeded,
                planeMap = toBecomeRiver
            };
            return floodPlane;
        }

        public struct PointCategory
        {
            public Func<(int x, int y), bool> isEqualZ;
            public Func<(int x, int y), bool> isBelowZ;
            public Func<(int x, int y), bool> isIgnoredPoint;
            public Func<(int x, int y), bool> isAboveZ;
        }

        /// <summary>
        /// </summary>
        /// <param name="startingPositions"></param>
        /// <param name="isEqualZ"></param>
        /// <param name="toBecomeRiver"></param>
        /// <returns></returns>
        private static (List<(int x, int y)> points, bool isLower)
            GetTouchingUnseen(IEnumerable<(int x, int y)> startingPositions, PointCategory pointCategory, BooleanMap toBecomeRiver, bool returnLower = false)
        {
            List<(int x, int y)> lowerHeight = new List<(int x, int y)>();
            List<(int x, int y)> equalHeight = new List<(int x, int y)>();

            HashSet<(int x, int y)> seenPoints = new HashSet<(int x, int y)>();

            var foundLower = false;
            foreach (var origin in startingPositions)
            {
                var neighbours = new (int x, int y)[] { Up(origin), Left(origin), Right(origin), Down(origin) };
                foreach (var neighbour in neighbours)
                {
                    if (toBecomeRiver.inBounds(neighbour.x, neighbour.y) &&
                        !toBecomeRiver.isMarked(neighbour.x, neighbour.y) &&
                        !seenPoints.Contains(neighbour) &&
                        !pointCategory.isIgnoredPoint(neighbour))
                    {
                        if (pointCategory.isEqualZ(neighbour) || (!returnLower && pointCategory.isBelowZ(neighbour)))
                        {
                            toBecomeRiver.setMarked(neighbour.x, neighbour.y);
                            equalHeight.Add(neighbour);
                        }
                        else if (pointCategory.isBelowZ(neighbour))
                        {
                            foundLower = true;
                            lowerHeight.Add(neighbour);
                        }
                    }
                    seenPoints.Add(neighbour);
                }
            }

            if (foundLower)
            {
                lowerHeight.ForEach(p =>
                {
                    Debug.Assert(!pointCategory.isIgnoredPoint(p));
                    Debug.Assert(pointCategory.isBelowZ(p));
                });
                return (lowerHeight, true);
            }

            return (foundLower ? lowerHeight : equalHeight, foundLower);
        }
    }
}
