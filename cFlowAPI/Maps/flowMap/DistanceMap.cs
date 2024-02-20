using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;
using Xamarin.Forms.Internals;

namespace application.Maps.flowMap;

public class DistanceMap : Map2d
{
    private (int x, int y) bounds;
    private IHeightMap heightMap;
    /// <summary>
    /// stores a xy offset and distance to the closest natural edge
    /// </summary>
    /// <param name="XOffset"></param>
    /// <param name="YOffset"></param>
    /// <param name="DistanceSquared"></param>
    public record struct DistancePoint(int distance, bool isSet)
    {
        public int DistanceSquared => distance;
    }

    private DistancePoint[][] distanceMap;

    public DistanceMap(IHeightMap heightMap)
    {
        this.bounds = heightMap.Bounds();
        this.heightMap = heightMap;
        this.distanceMap = new DistancePoint[bounds.x][];
        for (int i = 0; i < bounds.x; i++)
        {
            distanceMap[i] = new DistancePoint[bounds.y];
        }

    }

    private List<((int x, int y) point, DistancePoint distance)> MarkNaturalEdges()
    {
        if (Bounds() != heightMap.Bounds())
            throw new Exception($"map sizes dont match!: fMap:{Bounds()}, hMap:{heightMap.Bounds()}");
        //set Flow for all natural edges
        List<((int x, int y) point, DistancePoint distance)> outList =
            new List<((int x, int y) point, DistancePoint distance)>();
        foreach (var point in heightMap.iterator().Points())
        {
            if (DummyDimension.hasLowerNeighbours(point, heightMap))
                outList.Add((point, new DistancePoint(10, true)));
        }
        return outList;
    }

    public void CalculateFromHeightmap()
    {
        if (heightMap.Bounds() != Bounds())
            throw new Exception("illegal size given");

        var queue = new SortedQueue();
        var edges = MarkNaturalEdges();
        foreach (var origin in edges)
        {
            queue.TryInsert(origin.point, origin.distance.distance);
        }
        while (!queue.isEmpty())
        {
            var current = queue.Take();
            SetDistanceToEdge(current.point, new DistancePoint() { distance = current.value, isSet = true });
            var currentHeight = heightMap.GetHeight(current.point);

            var filter = ((int x, int y) p) =>
            {
                return inBounds(p.x, p.y) && !IsSet(p) && heightMap.GetHeight(p) >= currentHeight;
            };

            Point.Neighbours(current.point)
                .Where(filter)
                .Select(n => (n, current.value + 10))
                .ForEach(x => queue.TryInsert(x.n, x.Item2));

            Point.Diagonal(current.point)
                .Where(filter)
                .Select(n => (n, current.value + 14))
                .ForEach(x => queue.TryInsert(x.n, x.Item2));
        }

        foreach (var point in iterator().Points())
        {
            var d = GetDistanceOf(point);
            if ((IsSet(point) && d.distance == 0))
                Debug.WriteLine(":(");
        }
    }


    public bool IsSet((int x, int y) point)
    {
        return GetDistanceOf(point).isSet;
    }

    public SKBitmap ToCycleImage()
    {
        var (width, height) = Bounds();
        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque));
        //float ratio = int.MaxValue / Math.Max(Bounds().y, Bounds().x);
        var max = 0;

        foreach (var points in iterator().Points())
        {
            var origin = GetDistanceOf(points);
            if (origin.isSet)
            {
                var dist = (byte)((origin.DistanceSquared));
                bitmap.SetPixel(points.x, points.y, new SKColor(0, (byte)(255 - dist), dist));
            }
            else
            {
                bitmap.SetPixel(points.x, points.y, new SKColor(255, 0, 0));
            }
        }

        return bitmap;
    }

    public List<List<(int x, int y)>> FlowFrom((int x, int y) startPoint, List<(int x, int y)> lastUsed)
    {
        var startPointDist = GetDistanceOf(startPoint).DistanceSquared;
        var startPointHeight = heightMap.GetHeight(startPoint);
        var ns = Point.Neighbours(startPoint).Select(a => a);
            

        var lowerNeighbours = ns
            .Where(n => inBounds(n.x, n.y))
            .Where(n => heightMap.GetHeight(n) < startPointHeight);

        if (lowerNeighbours.Count() != 0)
        {
            return lowerNeighbours
                .Select(n => new List<(int x, int y)> { n })
                .ToList();
        }
        ns = ns.Where(n => !lastUsed.Contains(n));

        var flatNeighbours = ns
            .Where(n => inBounds(n.x, n.y))
            .Where(n => heightMap.GetHeight(n) == startPointHeight && GetDistanceOf(n).DistanceSquared < startPointDist)
            .ToList();

        List<List<(int x, int y)>> outList = flatNeighbours
            .Select(n => new List<(int x, int y)> { n })
            .ToList();

        bool canBeReachedFlat((int x, int y) point1, (int x, int y) point2)
        {
            Debug.Assert(point1 != point2 && Point.DistanceSquared(point1, point2) == 1);
            var b1 = inBounds(point1.x, point1.y);
            var b2 = inBounds(point2.x, point2.y);
            var d1 = GetDistanceOf(point1).distance;
            var d2 = GetDistanceOf(point2).distance;
            var d3 = heightMap.GetHeight(point1) == heightMap.GetHeight(point2);

            return
            lastUsed[0] != point1  &&
            lastUsed[0] != point2 &&
            (lastUsed.Count == 1 || (lastUsed[1] != point1 && lastUsed[1] != point2)) &&
            inBounds(point1.x, point1.y) &&
            inBounds(point2.x, point2.y) &&
            d1 < startPointDist &&
            d2 < startPointDist &&
             heightMap.GetHeight(point1) == heightMap.GetHeight(point2);
        }

        //diagonals with cross neighbours leading there.
        List<((int x, int y) p1, (int x, int y) p2)> xx =
        [
            (Point.Up(startPoint), Point.Right(Point.Up(startPoint))),
            (Point.Right(startPoint), Point.Down(Point.Right(startPoint))),
            (Point.Down(startPoint), Point.Left(Point.Down(startPoint))),
            (Point.Left(startPoint), Point.Up(Point.Left(startPoint))),

            (Point.Up(startPoint), Point.Left(Point.Up(startPoint))),
            (Point.Right(startPoint), Point.Up(Point.Right(startPoint))),
            (Point.Down(startPoint), Point.Right(Point.Down(startPoint))),
            (Point.Left(startPoint), Point.Down(Point.Left(startPoint)))
        ];


        var acceptedDiag = xx
            .Where(ps => canBeReachedFlat(ps.p1, ps.p2))
            .Select(ps =>
                //TODO: include ps.p1 here, after we made sure that river doesnt try to revisit any points.
            new List<(int x, int y)>() {  ps.p2 })
            .ToList();

        outList.AddRange(acceptedDiag);
        //  if (acceptedDiag.Count > 0)
        //      return acceptedDiag;
        return outList;
    }



    public void SetDistanceToEdge((int x, int y) point, DistancePoint distance)
    {
        this.distanceMap[point.x][point.y] = distance;
    }

    public DistancePoint GetDistanceOf((int x, int y) point)
    {
        var val = distanceMap[point.x][point.y];
        return val;
    }

    public (int x, int y) Bounds()
    {
        return bounds;
    }

    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < Bounds().x && y >= 0 && y < Bounds().y;
    }

    public IMapIterator<(int x, int y)> iterator()
    {
        return new Map2dIterator(Bounds());
    }
}