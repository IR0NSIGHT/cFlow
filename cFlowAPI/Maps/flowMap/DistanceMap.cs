using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;

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
    public record struct DistancePoint(short XOffset, short YOffset, bool isSet)
    {
        public short DistanceSquared => (short)(XOffset * XOffset + YOffset * YOffset);
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
                outList.Add((point, new DistancePoint(0, 0, true)));
        }
        return outList;
    }

    public void CalculateFromHeightmap()
    {
        if (heightMap.Bounds() != Bounds())
            throw new Exception("illegal size given");

        var origins = MarkNaturalEdges();
        foreach (var origin in origins)
        {
            SetDistanceToEdge(origin.point, origin.distance);
        }
        while (origins.Count != 0)
        {
            origins = ExpandDistancesFor(origins);
        }
    }


    /// <summary>
    /// calculate distances for the neighbours of points
    /// </summary>
    public List<((int x, int y) point, DistancePoint distance)> ExpandDistancesFor(List<((int x, int y) point, DistancePoint distance)> points)
    {
        var outList = new ((int x, int y) point, DistancePoint distance)[points.Count * 4];
        int outIdx = 0;
        foreach (var origin in points)
        {
            var ns = GetNeighbourDistances(origin.point, origin.distance);
            var originHeight = heightMap.GetHeight(origin.point);
            foreach (var n in ns)
            {
                // we only care about higher/equal neighbours => those flow towards us
                if (heightMap.GetHeight(n.point) < originHeight)
                    continue;
                var existinValue = GetDistanceOf(n.point);

                //found a better value for this point
                if (existinValue.isSet && existinValue.DistanceSquared > n.distance.DistanceSquared)
                {
                    //overwrite point/distance in outList
                    for (int i = 0; i < outList.Length; i++)
                    {
                        var entry = outList[i];
                        if (entry.point == n.point)
                        {
                            entry.distance = n.distance;
                            outList[i] = entry;
                            break;
                        }
                    }
                    SetDistanceToEdge(n.point,n.distance);
                }
                else if (!existinValue.isSet)
                {
                    SetDistanceToEdge(n.point, n.distance);
                    outList[outIdx++] = n;
                }


            }
        }
        Array.Resize(ref outList, outIdx);
        return outList .ToList();
    }

    private List<((int x, int y) point, DistancePoint distance)> GetNeighbourDistances(
        (int x, int y) origin, DistancePoint originDistance)
    {
        var outList =
            new List<((int x, int y) point, DistancePoint distance)>();
        foreach (var n in Point.Neighbours(origin))
        {
            if (inBounds(n.x, n.y))
            {
                var delta = (n.x - origin.x, n.y - origin.y);
                var offset = ((short)(originDistance.XOffset + delta.Item1), (short)(originDistance.YOffset + delta.Item2));
                outList.Add((
                    n,
                    new DistancePoint(
                        offset.Item1,
                        offset.Item2, true)));
            }
        }
        return outList;
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
            var dist = GetDistanceOf(points).DistanceSquared;
            bitmap.SetPixel(points.x, points.y, ((uint)(dist) | 0xFF000000));
            if (dist > max)
                max = dist;
        }

        return bitmap;
    }

    public List<(int x, int y)> FlowFrom((int x, int y) point)
    {
        var pointDistance = GetDistanceOf(point).DistanceSquared;
        List<(int x, int y)> outList = new();
        foreach (var n in Point.Neighbours(point))
        {
            if (inBounds(n.x, n.y) && GetDistanceOf(n).DistanceSquared < pointDistance)
            {
                outList.Add(n);
            }
        }
        return outList;
    }

    public void SetDistanceToEdge((int x, int y) point, DistancePoint distance)
    {
        this.distanceMap[point.x][point.y] = distance;
    }

    public DistancePoint GetDistanceOf((int x, int y) point)
    {
        return distanceMap[point.x][point.y];
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