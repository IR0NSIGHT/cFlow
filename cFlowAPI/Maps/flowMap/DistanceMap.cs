using System.Diagnostics;
using cFlowApi.Heightmap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cFlowAPI.Maps.Shader;
using ComputeSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TerraFX.Interop.Windows;
using System.IO;

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
        this.distanceMap = DummyDimension.arrayOfSize<DistancePoint>(Bounds().x, Bounds().y);
    }

    public uint[,] toGpuData()
    {
        uint[,] result = new uint[Bounds().y, Bounds().x];
        for (int y = 0; y < Bounds().y; y++)
        {
            for (int x = 0; x < Bounds().x; x++)
            {
                result[y, x] = (uint)GetDistanceOf((x, y)).distance;
            }
        }

        return result;
    }
    private List<((int x, int y) point, DistancePoint distance)> MarkNaturalEdges()
    {
        if (Bounds() != heightMap.Bounds())
            throw new Exception($"map sizes dont match!: fMap:{Bounds()}, hMap:{heightMap.Bounds()}");
        //set Flow for all natural edges
        uint[,] pointData = ((DummyDimension)heightMap).ToGPUdata();

        using ReadOnlyTexture2D<uint> heightmap = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D<uint>(pointData);
        using ReadWriteTexture2D<uint> flowmap = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(heightmap.Width, heightmap.Height);

        var shader = new FindEdgeDistanceShader(heightmap, flowmap);
        GraphicsDevice.GetDefault().For(heightmap.Width, heightmap.Height, shader);
        flowmap.CopyTo(pointData);
        this.FromGPUdata(pointData);



        //collect all marked points
        List<((int x, int y) point, DistancePoint distance)> outList =
            new List<((int x, int y) point, DistancePoint distance)>();
        foreach (var point in heightMap.iterator().Points())
        {
            if (IsSet(point))
                outList.Add((point, GetDistanceOf(point)));
        }
        return outList;
    }

    public void FromGPUdata(uint[,] data)
    {
        for (int y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                SetDistanceToEdge((x, y), new DistancePoint((int)data[y, x], data[y, x] != 0));
            }
        }
    }

    public void CalculateFromHeightmap()
    {
        if (heightMap.Bounds() != Bounds())
            throw new Exception("illegal size given");

        var queue = new SortedQueue();
        var edges = MarkNaturalEdges();

        var sw = Stopwatch.StartNew();
        uint[,] pointData = this.toGpuData();
        Debug.WriteLine($"transforming distance data took: {sw.ElapsedMilliseconds} millis.");
        sw.Restart();
        uint[,] heightData = ((DummyDimension)this.heightMap).ToGPUdata();
        Debug.WriteLine($"transforming height data took: {sw.ElapsedMilliseconds} millis.");
        sw.Restart();

        var shader = FromMaps(pointData, heightData);
        Debug.WriteLine($"loading data to GPU took: {sw.ElapsedMilliseconds} millis.");
        sw.Restart();


        var changed = true;
        for (int i = 0; i < Math.Max(Bounds().x, Bounds().y); i++)
        {
            if (!changed)
                break;

            shader.changed.CopyFrom(new int[1]);
            GraphicsDevice.GetDefault().For(Bounds().x, Bounds().y, shader);

            changed = didChange(shader);
            Debug.WriteLine($"shader iteration {i} took: {sw.ElapsedMilliseconds} millis.");
            sw.Restart();

        }

        this.FromGPUdata(shader.distanceMap.ToArray());
    }

    public Bitmap toImage()
    {
        var distanceData = toGpuData();
        using var distanceTexture = GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D(distanceData);
        using var outputRgba32 = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<Rgba32, float4>(distanceTexture.Width, distanceTexture.Height);
        var shader = new DistanceToImageShader(distanceTexture, outputRgba32);
        GraphicsDevice.GetDefault().For(distanceTexture.Height, distanceTexture.Width, shader);

        Stream stream = new MemoryStream();
        outputRgba32.Save(stream, ImageFormat.Png);
        using (Image image = Image.FromStream(stream))
        {
            var bmp = new Bitmap(image);
            return bmp;
        }
    }

    public List<(int x, int y)> AdvanceFromPoint((int x, int y) startFlow)
    {
        var ownHeight = this.heightMap.GetHeight(startFlow);
        var ownDistance = GetDistanceOf(startFlow).distance;
        var outList = new List<(int x, int y)>();
        outList.AddRange(Point.Diagonal(startFlow));
        outList.AddRange(Point.Neighbours(startFlow));
        var cliffs = outList.Where(p => heightMap.GetHeight(p) < ownHeight);
        if (cliffs.Any())
            return cliffs.ToList();

        return
            outList.Where(p => heightMap.GetHeight(p) == ownHeight)
            .Where(p => GetDistanceOf(p).distance < ownDistance)
            .ToList();
    }


    public bool IsSet((int x, int y) point)
    {
        return GetDistanceOf(point).isSet;
    }

    //public SKBitmap ToCycleImage()
    //{
    //    var (width, height) = Bounds();
    //    SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque));
    //    //float ratio = int.MaxValue / Math.Max(Bounds().y, Bounds().x);
    //    var max = 0;
    //
    //    foreach (var points in iterator().Points())
    //    {
    //        var origin = GetDistanceOf(points);
    //        if (origin.isSet)
    //        {
    //            var dist = (byte)((origin.DistanceSquared));
    //            bitmap.SetPixel(points.x, points.y, new SKColor(0, (byte)(255 - dist), dist));
    //        }
    //        else
    //        {
    //            bitmap.SetPixel(points.x, points.y, new SKColor(255, 0, 0));
    //        }
    //    }
    //
    //    return bitmap;
    //}

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
            lastUsed[0] != point1 &&
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
            new List<(int x, int y)>() { ps.p2 })
            .ToList();

        outList.AddRange(acceptedDiag);
        //  if (acceptedDiag.Count > 0)
        //      return acceptedDiag;
        return outList;
    }



    public void SetDistanceToEdge((int x, int y) point, DistancePoint distance)
    {
        this.distanceMap[point.y][point.x] = distance;
    }

    public DistancePoint GetDistanceOf((int x, int y) point)
    {
        var val = distanceMap[point.y][point.x];
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

    public static ExpandDistanceShader FromMaps(uint[,] distanceMap, uint[,] heightMap)
    {
        var distanceData = GraphicsDevice.GetDefault()
            .AllocateReadWriteTexture2D<uint>(distanceMap);

        var heightTexture = GraphicsDevice.GetDefault()
            .AllocateReadOnlyTexture2D(heightMap);

        var changedArr = new int[1];
        var changed = GraphicsDevice.GetDefault()
            .AllocateReadWriteBuffer<int>(changedArr);

        return new ExpandDistanceShader(distanceData, heightTexture, changed); ;
    }

    public static bool didChange(ExpandDistanceShader shader)
    {
        var result = new int[1];
        shader.changed.CopyTo(result);
        return result[0] != 0;
    }
}