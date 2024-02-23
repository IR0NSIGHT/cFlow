
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

        public List<(int x, int y)> FloodArea((int x, int y) start, RiverMap targetRiverMap, int maxDepth = 10, int maxSurfaceBeforeExceeded = 100000)
        {
            int startZ = _heightMap.GetHeight(start);

            var lakeMap = new BooleanMap(targetRiverMap.Bounds());
            List<(int x, int y)> currentOuterMost = [start];
            for (int i = 0; i < maxDepth; i++)
            {
                var maxZ = startZ + i;
                //mark outermost ring on lakemap. planecollection will ignore this ring when finding neighbours
                foreach (var point in currentOuterMost)
                {
                    lakeMap.setMarked(point.x, point.y);
                }

                var (outerMost, toBeRiver, exceeded, escapePoints) =
                    collectPlaneAtOrBelow(
                        currentOuterMost,
                        maxZ,
                        p => lakeMap.isMarked(p.x, p.y),
                        maxSurfaceBeforeExceeded,
                        true);

                if (exceeded)
                    return new List<(int x, int y)>();

                currentOuterMost = outerMost;
                foreach (var p in toBeRiver.IterateMarked())
                {
                    targetRiverMap.SetAsRiver(p.x, p.y);
                }

                if (escapePoints.Count != 0)
                {
                    return escapePoints;
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
        public (List<(int x, int y)> outerMostRing, BooleanMap toBecomeRiver, bool exceededMax, List<(int x, int y)> lowerEscapePoints) collectPlaneAtOrBelow(List<(int x, int y)> startingPositions, int maxZ, Func<(int x, int y), bool> isIgnoredPoint, int maxSurfaceBeforeExceeded = -1, bool abortOnLowerFound = false)
        {
            BooleanMap toBecomeRiver = new BooleanMap(_heightMap.Bounds());
            Func<(int x, int y), bool> isEqualZ = pos =>
                _heightMap.GetHeight(pos) == maxZ;
            Func<(int x, int y), bool> isBelowZ = pos =>
                _heightMap.GetHeight(pos) < maxZ;

            List<(int x, int y)> outerMostRing = [];

            var nextPositions = startingPositions;
            foreach (var (x, y) in startingPositions)
            {
                toBecomeRiver.setMarked(x, y);
            }

            while (true)
            {
                var (nextRing, didFindLower) = GetTouchingUnseen(nextPositions, isEqualZ, isBelowZ, toBecomeRiver, isIgnoredPoint, abortOnLowerFound);
                //ring has found everything, nothing more to do
                if (nextRing.Count == 0)
                    break;

                if (didFindLower && abortOnLowerFound)   //we found an escape from this plane => return all points that were flooded and the escape points.
                {
                    return ([],
                        toBecomeRiver,
                        false,
                        nextRing);
                }

                nextPositions = nextRing;

                foreach (var pos in nextRing)
                {
                    if (isBorderPoint(pos, p => !isEqualZ(p) && !isBelowZ(p)))//TODO is this check not guaranteed implicitly by GetTouchingUnseen?
                        outerMostRing.Add(pos);
                }

                //ring has exceeded maximum size
                if (maxSurfaceBeforeExceeded != -1 && toBecomeRiver.getMarkedAmount() > maxSurfaceBeforeExceeded)
                {
                    break;
                }
            }

            return (outerMostRing, toBecomeRiver, maxSurfaceBeforeExceeded != -1 && toBecomeRiver.getMarkedAmount() > maxSurfaceBeforeExceeded, new List<(int x, int y)>());
        }

        /// <summary>
        /// </summary>
        /// <param name="startingPositions"></param>
        /// <param name="isEqualZ"></param>
        /// <param name="toBecomeRiver"></param>
        /// <returns></returns>
        private static (List<(int x, int y)> points, bool isLower)
            GetTouchingUnseen(IEnumerable<(int x, int y)> startingPositions, Func<(int x, int y), bool> isEqualZ, Func<(int x, int y), bool> isBelowZ, BooleanMap toBecomeRiver, Func<(int x, int y), bool> isIgnoredPoint, bool returnLower = false)
        {
            List<(int x, int y)> lowerHeight = new List<(int x, int y)>();
            List<(int x, int y)> equalHeight = new List<(int x, int y)>();

            var foundLower = false;
            foreach (var origin in startingPositions)
            {
                var neighbours = new (int x, int y)[] { Up(origin), Left(origin), Right(origin), Down(origin) };
                foreach (var neighbour in neighbours)
                {
                    if (toBecomeRiver.inBounds(neighbour.x, neighbour.y) &&
                        !toBecomeRiver.isMarked(neighbour.x, neighbour.y) &&
                        !isIgnoredPoint(neighbour))
                    {
                        if (isEqualZ(neighbour) || (!returnLower && isBelowZ(neighbour)))
                        {
                            toBecomeRiver.setMarked(neighbour.x, neighbour.y);
                            equalHeight.Add(neighbour);
                        }
                        else if (isBelowZ(neighbour))
                        {
                            foundLower = true;
                            lowerHeight.Add(neighbour);
                            toBecomeRiver.setMarked(neighbour.x, neighbour.y);
                        }
                    }
                }
            }
            return (foundLower ? lowerHeight : equalHeight, foundLower);
        }
    }
}
