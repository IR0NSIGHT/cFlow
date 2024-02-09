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
                outList.Add((point, new DistancePoint(10,true)));
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
            if (IsSet(current.point))
                continue;
            SetDistanceToEdge(current.point, new DistancePoint(){ distance = current.value, isSet = true});
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
                var dist = (byte)((origin.DistanceSquared % 25) * 10);
                bitmap.SetPixel(points.x, points.y, new SKColor(0, (byte)(255 - dist), dist));
            }
            else
            {
                bitmap.SetPixel(points.x, points.y, new SKColor(255, 0, 0));
            }


        }

        return bitmap;
    }

    public List<(int x, int y)> FlowFrom((int x, int y) point)
    {
        var pointDistance = GetDistanceOf(point).DistanceSquared;
        var pointHeight = heightMap.GetHeight(point);
        List<(int x, int y)> outList = new();
        var ns = Point.Neighbours(point);
        var accepted = ns.Where(n => inBounds(n.x, n.y))
            .Where(n => heightMap.GetHeight(n) <= pointHeight)
            .Where(n => GetDistanceOf(n).DistanceSquared < pointDistance)
            .ToList();
        if (accepted.Count == 0)
        {
            var debugList = ns.Select(n => (n, heightMap.GetHeight(n), GetDistanceOf(n).distance)).ToList();
            Debug.WriteLine("on no");

        }
        return accepted.ToList();
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