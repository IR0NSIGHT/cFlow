using application.Maps;
using SkiaSharp;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Threading;

public class SimpleFlowMap : IFlowMap
{
    private IFlowMap.Flow[][] flowMap;
    private int[][] cycleMap;

    public SimpleFlowMap((int x, int y) dimension)
    {
        flowMap = emptyFlowMap(dimension);
        cycleMap = emptyCycleMap(dimension);
        FillWithUnknown(this);
    }

    private static int[][] emptyCycleMap((int x, int y) dimension)
    {
        var map = new int[dimension.x][];
        for (int i = 0; i < dimension.x; i++)
        {
            map[i] = new int[dimension.y];
        }

        return map;
    }

    private static IFlowMap.Flow[][] emptyFlowMap((int x, int y) dimensions)
    {
        IFlowMap.Flow[][] flowMap = new IFlowMap.Flow[dimensions.x][];
        for (int i = 0; i < dimensions.x; i++)
        {
            flowMap[i] = new IFlowMap.Flow[dimensions.y];
        }

        return flowMap;
    }

    public SimpleFlowMap FromHeightMap(IHeightMap heightMap)
    {
        FillWithUnknown(this);
        CalculateFlowFromHeightMap(heightMap, this);
        return this;
    }

    private static void FillWithUnknown(IFlowMap flowMap)
    {
        //set everything to unknown
        foreach (var point in flowMap.iterator().Points())
        {
            flowMap.SetFlow(point,
                new IFlowMap.Flow(
                    true, false, false, false, false
                ), Int32.MaxValue);
        }
    }

    /// <summary>
    /// will put flow values on all blocks that have a lower neighbour
    /// will flow towards ALL lower neighbours.
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="flowMap"></param>
    public static void ApplyNaturalEdgeFlow(IHeightMap heightMap, IFlowMap flowMap)
    {
        if (flowMap.Bounds() != heightMap.Bounds())
            throw new Exception($"map sizes dont match!: fMap:{flowMap.Bounds()}, hMap:{heightMap.Bounds()}");
        //set Flow for all natural edges
        foreach (var point in heightMap.iterator().Points())
        {
            IFlowMap.Flow flow = pointFlowByHeight(point, heightMap);
            if (!flow.Unknown)
                flowMap.SetFlow(point, flow, 0);
        }
    }

    /// <summary>
    /// point will try to find a neighbour with a flow.
    /// returns default flow if no valid neighbour was found
    /// </summary>
    /// <param name="point"></param>
    /// <param name="flowMap"></param>
    /// <returns>(changeHappend, newFlow)</returns>
    private static (bool, IFlowMap.Flow) NeighbourFlowOrDefault(int x, int y, IFlowMap flowMap, IHeightMap heightMap,
        IFlowMap.Flow defaultF)
    {
        Point thisP = new Point(x, y, -1);
        Func<(int x, int y), Boolean> canIFlowTo = (pos) =>
            heightMap.GetHeight(x, y) >= heightMap.GetHeight(pos.x, pos.y);


        Func<(int x, int y), Boolean> hasFlow = (pos) =>
        {
            if (!flowMap.inBounds(pos.x, pos.y))
                return false;
            if (!canIFlowTo(pos))
            {
                return false;
            }

            var flow = flowMap.GetFlow(pos);

            if (flow.Unknown)
                return false;
            return (flow.Left || flow.Right || flow.Up || flow.Down);
        };


        //check if neighbours in specific direction have flow
        bool flowLeft = hasFlow(thisP.Left());
        bool flowRight = hasFlow(thisP.Right());
        bool flowUp = hasFlow(thisP.Up());
        bool flowDown = hasFlow(thisP.Down());
        if (flowLeft || flowRight || flowUp || flowDown)
        {
            return (
                true,
                new IFlowMap.Flow(false, flowUp, flowDown, flowLeft, flowRight));
        }

        return (false, defaultF);
    }

    private static bool insideReducedBounds((int x, int y) pos, (int x, int y) dims)
    {
        var positive = pos.x - 1 >= 0 && pos.y - 1 >= 0;
        var negative = pos.x + 1 < dims.x && pos.y + 1 < dims.y;
        return positive && negative;
    }

    private static (int x, int y)[] collectNonHigherNeighbours(IFlowMap.PointFlow[] previousChanged,
        SimpleFlowMap flowMap, BooleanMap seenMap, IHeightMap heightMap)
    {
        //collect all neighbours of the previous run
        var currentIdx = 0;
        var candidates = new (int x, int y)[previousChanged.Length * 4];

        var dims = flowMap.Bounds();
        Func<(int, int), Boolean> safeBounds = ((int x, int y) pos) => insideReducedBounds(pos, dims);

        //TODO check if candidate already exists
        foreach (var previousPoint in previousChanged)
        {
            var (x, y) = (previousPoint.X, previousPoint.Y);
            (int x, int y)[] news = { Point.Up(x, y), Point.Left(x, y), Point.Right(x, y), Point.Down(x, y) };
            var pointInReducedBounds = safeBounds((x, y));
            Func<(int, int), bool> canFlowTo = ((int x, int y) p) =>
            {
                return heightMap.GetHeight(previousPoint.X, previousPoint.Y) <= heightMap.GetHeight(p.x, p.y);
            };
            foreach (var p in news)
            {
                if ((pointInReducedBounds || flowMap.inBounds(p.x, p.y)) && !seenMap.isMarked(p.x, p.y) &&
                    canFlowTo((p.x, p.y)))
                {
                    candidates[currentIdx++] = (p.x, p.y);
                    seenMap.setMarked(p.x, p.y);
                }
            }

            ;
        }

        ;
        Array.Resize(ref candidates, currentIdx);
        return candidates;
    }

    /// <summary>
    /// will find flow for given origins, flowing towards neighbours that have a flow
    /// only expands if neighbour doesnt have to flow uphill towards origin
    /// </summary>
    /// <param name="origins"></param>
    /// <param name="flowMap"></param>
    /// <param name="heightMap"></param>
    /// <returns></returns>
    private static IFlowMap.PointFlow[] calculateExpandedFlowFor((int x, int y)[] origins, SimpleFlowMap flowMap,
        IHeightMap heightMap)
    {
        var changedCandidates = new IFlowMap.PointFlow[origins.Length];
        int changedIdx = 0;
        foreach (var point in origins)
        {
            var f = flowMap.GetFlow(point);
            var (changed, newFlow) = NeighbourFlowOrDefault(point.x, point.y, flowMap, heightMap, f);
            if (changed)
            {
                changedCandidates[changedIdx++] = new IFlowMap.PointFlow(point.x, point.y, newFlow);
            }
        }

        Array.Resize(ref changedCandidates, changedIdx);
        return changedCandidates;
    }


    /// <summary>
    /// if a point with unknown flow has neighbours with a flow, the point will flow to these neighbours.
    /// mutates flowMap
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="flowMap"></param>
    /// <returns>change occured</returns>
    private static (bool changed, IFlowMap.PointFlow[] changedPoints) ExpandExistingFlow(SimpleFlowMap flowMap,
        IHeightMap heightMap, IFlowMap.PointFlow[] previousChanged, BooleanMap seenMap, int cycle)
    {
        var candidates = collectNonHigherNeighbours(previousChanged, flowMap, seenMap, heightMap);
        var changedCandidates = calculateExpandedFlowFor(candidates, flowMap, heightMap);
        bool changeOccured = changedCandidates.Length != 0;

        //apply all changes to flowmap
        if (changeOccured)
        {
            foreach (var flowPoint in changedCandidates)
            {
                flowMap.SetFlow((flowPoint.X, flowPoint.Y), flowPoint.Flow, cycle);
            }
        }

        return (changeOccured, changedCandidates);
    }


    /// <summary>
    /// will mutate flowmap, calculating flow based on the heightmap provided.
    /// flow will go from high to low points
    /// blocks without natural flow will be written as Unknown
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="flowMap"></param>
    public static void CalculateFlowFromHeightMap(IHeightMap heightMap, SimpleFlowMap flowMap)
    {
        ApplyNaturalEdgeFlow(heightMap, flowMap);

        var coloredFlow = SimpleFlowMap.ToColorImage(flowMap, p =>
        {
            if (!p.Unknown) return new SKColor(255, 0, 0);
            else return new SKColor(0, 0, 0);
        });
        ImageApi.SaveBitmapAsPng(coloredFlow, "C:\\Users\\Max1M\\\\OneDrive\\Bilder\\flowAfterNatural.png");

        //get all existing flows
        var edges = new List<IFlowMap.PointFlow>();
        foreach (var point in flowMap.iterator().Points())
        {
            if (!flowMap.GetFlow(point).Unknown)
                edges.Add(new IFlowMap.PointFlow(point.x, point.y, flowMap.GetFlow(point)));
        }

        var changed = true;
        var seenMap = new BooleanMap(flowMap.Bounds());

        //first candidates are the natural edges
        var previousCandidated = edges.ToArray();
        foreach (var candidated in previousCandidated)
            seenMap.setMarked(candidated.X, candidated.Y);
        int cycle = 0;
        while (changed)
        {
            var expanded = ExpandExistingFlow(flowMap, heightMap, previousCandidated, seenMap, cycle);
            changed = expanded.changed;
            previousCandidated = expanded.changedPoints;
            cycle++;
        }
    }

    public SKBitmap ToCycleImage()
    {
        var (width, height) = Bounds();
        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque));
        float ratio = int.MaxValue / Math.Max(Bounds().y, Bounds().x);
        foreach (var points in iterator().Points())
        {
            bitmap.SetPixel(points.x, points.y, ((uint)(GetCylce(points) * ratio)));
        }

        return bitmap;
    }

    public static SKBitmap ToImage(IFlowMap flowMap)
    {
        var (width, height) = flowMap.Bounds();
        byte[] pixelData = new byte[width * height];

        foreach (var points in flowMap.iterator().Points())
        {
            pixelData[points.y * width + points.x] = FlowTranslation.FlowToGray8(flowMap.GetFlow(points));
        }

        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Gray8, SKAlphaType.Opaque));
        IntPtr pixelsPointer = Marshal.UnsafeAddrOfPinnedArrayElement(pixelData, 0);
        bitmap.SetPixels(pixelsPointer);
        return bitmap;
    }

    public static SKBitmap ToColorImage(IFlowMap flowMap, Func<IFlowMap.Flow, SKColor> flowToColor)
    {
        var (width, height) = flowMap.Bounds();
        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque));
        foreach (var points in flowMap.iterator().Points())
        {
            bitmap.SetPixel(points.x, points.y, flowToColor(flowMap.GetFlow(points)));
        }

        return bitmap;
    }

    private static bool isLowerThan((int x, int y) pos, IHeightMap heightMap, short height, bool safeBounds)
    {
        return (safeBounds || heightMap.inBounds(pos.x, pos.y)) && heightMap.GetHeight(pos.x, pos.y) < height;
    }

    /// <summary>
    /// will flow towards all lower neighbours. if ZERO lower neighbours are found, flow will be UNKNOWN
    /// </summary>
    /// <param name="p"></param>
    /// <param name="heightMap"></param>
    /// <returns></returns>
    private static IFlowMap.Flow pointFlowByHeight((int X, int Y) p, IHeightMap heightMap)
    {
        short height = heightMap.GetHeight(p.X, p.Y);
        bool safeBounds = insideReducedBounds((p.X, p.Y), heightMap.Bounds());
        bool left = isLowerThan(Point.Left(p.X, p.Y), heightMap, height, safeBounds);
        bool right = isLowerThan(Point.Right(p.X, p.Y), heightMap, height, safeBounds);
        bool up = isLowerThan(Point.Up(p.X, p.Y), heightMap, height, safeBounds);
        bool down = isLowerThan(Point.Down(p.X, p.Y), heightMap, height, safeBounds);
        if (!(left || right || up || down))
            return new IFlowMap.Flow(true, false, false, false, false);
        return new IFlowMap.Flow(false, up, down, left, right);
    }

    public void SetCylce((int x, int y) point, int cycle)
    {
        cycleMap[point.x][point.y] = cycle;
    }

    public List<(int x, int y)> FollowFlow((int x, int y) point)
    {
        IFlowMap.Flow flow = GetFlow(point);
        List<(int x, int y)> nextFlow = new();
        if (flow.Up)
            nextFlow.Add(Point.Up(point.x, point.y));
        if (flow.Down)
            nextFlow.Add(Point.Down(point.x, point.y));
        if (flow.Left)
            nextFlow.Add(Point.Left(point.x, point.y));
        if (flow.Right)
            nextFlow.Add(Point.Right(point.x, point.y));

        return nextFlow;
    }

    public (int x, int y) Bounds()
    {
        return (flowMap.Length, flowMap[0].Length);
    }

    public IFlowMap.Flow GetFlow((int x, int y) point)
    {
        return flowMap[point.x][point.y];
    }

    public void SetFlow((int x, int y) point, IFlowMap.Flow flow, int cycle)
    {
        flowMap[point.x][point.y] = flow;
        SetCylce(point, cycle);
    }

    public int GetCylce((int x, int y) point)
    {
        return cycleMap[point.x][point.y];
    }

    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < Bounds().x && y >= 0 && y < Bounds().y;
        ;
    }

    public IMapIterator<(int x, int y)> iterator()
    {
        return new Map2dIterator(Bounds());
    }
}