
using application.Maps;
using static Point;

namespace cFlowAPI.Maps.riverMap
{
    public class FloodTool
    {
        private IHeightMap _heightMap;
        public FloodTool(IHeightMap heightMap)
        {
            _heightMap = heightMap;
        }

        public void FloodArea((int x, int y) start)
        {
            int startZ = _heightMap.GetHeight(start);

        }

        /// <summary>
        /// test if this point has a neighbour that is not belowZ
        /// => point is touching the border of the plane defined by belowZ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isBelowZ"></param>
        /// <returns></returns>
        private bool isBorderPoint((int x, int y) pos, Func<(int x, int y), bool> isBelowZ) {
            var neighs = new (int x, int y)[] { Up(pos), Down(pos), Left(pos), Right(pos) };
            foreach (var neigh in neighs)
            {
                if (_heightMap.inBounds(neigh.x, neigh.y) &&  !isBelowZ(neigh))
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
        public (List<(int x, int y)> outerMostRing, BooleanMap seen, bool exceededMax) collectPlaneAtOrBelow(List<(int x, int y)> startingPositions, int maxZ, int maxSurfaceBeforeExceeded = -1)
        {
            BooleanMap seenMap = new BooleanMap(_heightMap.Bounds());
            Func<(int x, int y), bool> isBelowZ = pos =>
                _heightMap.GetHeight(pos) <= maxZ;

            List<(int x, int y)> outerMostRing = new List<(int x, int y)>();

            var nextPositions = startingPositions;
            foreach (var pos in startingPositions)
            {
                seenMap.setMarked(pos.x, pos.y);
            }

            while (true)
            {
                var nextRing = GetTouchingUnseen(nextPositions, isBelowZ, seenMap);
                //ring has found everything, nothing more to do
                if (nextRing.Count() == 0)
                    break;

                nextPositions = nextRing;

                foreach (var pos in nextRing)
                {
                    if (isBorderPoint(pos, isBelowZ))
                        outerMostRing.Add(pos);
                }

                //ring has exceeded maximum size
                if (maxSurfaceBeforeExceeded != -1 && seenMap.getMarkedAmount() > maxSurfaceBeforeExceeded)
                {
                    break;
                }
            }

            return (outerMostRing, seenMap, maxSurfaceBeforeExceeded != -1 && seenMap.getMarkedAmount() > maxSurfaceBeforeExceeded);
        }

        /// <summary>
        /// </summary>
        /// <param name="startingPositions"></param>
        /// <param name="isBelowEqualZ"></param>
        /// <param name="seenMap"></param>
        /// <returns></returns>
        private static List<(int x, int y)> GetTouchingUnseen(IEnumerable<(int x, int y)> startingPositions, Func<(int x, int y), bool> isBelowEqualZ, BooleanMap seenMap)
        {
            List<(int x, int y)> nextPositions = new List<(int x, int y)>();
            foreach (var origin in startingPositions)
            {
                var neighbours = new (int x, int y)[] { Up(origin), Left(origin), Right(origin), Down(origin) };
                foreach (var neighbour in neighbours)
                {
                    if (seenMap.inBounds(neighbour.x, neighbour.y) && !seenMap.isMarked(neighbour.x, neighbour.y) && isBelowEqualZ(neighbour))
                    {
                        seenMap.setMarked(neighbour.x, neighbour.y);
                        nextPositions.Add(neighbour);
                    }
                }
            }
            return nextPositions;
        }
    }
}
