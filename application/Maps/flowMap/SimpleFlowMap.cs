using SkiaSharp;
using System.Runtime.InteropServices;

public class SimpleFlowMap : IFlowMap
{

    private IFlowMap.Flow[][] flowMap;




    public SimpleFlowMap((int x, int y) dimension)
    {
        flowMap = emptyFlowMap(dimension);
        FillWithUnknown(this);
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
        foreach (var point in flowMap.GetPoints())
        {
            flowMap.SetFlow(point.X, point.Y,
                new IFlowMap.Flow(
                    true, false, false, false, false
                    ));
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
        //set Flow for all natural edges
        foreach (var point in heightMap.iterator().Points())
        {
            if (!flowMap.inBounds(point.x, point.y))
                throw new Exception($"illegal point!: {point}");
            IFlowMap.Flow flow = pointFlowByHeight(point, heightMap);
            flowMap.SetFlow(point.x, point.y, flow);
        }
    }

    /// <summary>
    /// point will try to find a neighbour with a flow.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="flowMap"></param>
    /// <returns>(changeHappend, newFlow)</returns>
    private static (bool, IFlowMap.Flow) PointFollowNeighbours(IFlowMap.PointFlow point, IFlowMap flowMap, IHeightMap heightMap)
    {
        if (!point.Flow.Unknown)
            return (false, point.Flow);

        var (x, y) = (point.X, point.Y);
        Point thisP = new Point(x, y, -1);

        Func<(int x, int y), Boolean> canIFlowTo = (pos) =>
            heightMap.GetHeight(point.X, point.Y) >= heightMap.GetHeight(pos.x, pos.y);


        Func<(int x, int y), Boolean> hasFlow = (pos) =>
        {

            if (!flowMap.inBounds(pos.x, pos.y))
                return false;
            if (!canIFlowTo(pos))
            {
                return false;
            }
            var flow = flowMap.GetFlow(pos.x, pos.y).Flow;

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


        return (false, point.Flow);

    }

    private static bool insideReducedBounds((int x, int y) pos, (int x, int y) dims)
    {
        return pos.x - 1 >= 0 && pos.y - 1 >= 0 && pos.x + 1 < dims.x && pos.y + 1 < dims.y;
    }

    private static (int x, int y)[] collectNeighbours(IFlowMap.PointFlow[] previousChanged, SimpleFlowMap flowMap, bool[][] seenMap)
    {
        //collect all neighbours of the previous run
        var currentIdx = 0;
        var candidates = new (int x, int y)[previousChanged.Length * 4];


        var dims = flowMap.getDimensions();
        Func<(int, int), Boolean> safeBounds = ((int x, int y) pos) => insideReducedBounds(pos, dims);

        //TODO check if candidate already exists
        foreach (var previousPoint in previousChanged)
        {
            var (x, y) = (previousPoint.X, previousPoint.Y);
            (int x, int y)[] news = { Point.Up(x, y), Point.Left(x, y), Point.Right(x, y), Point.Down(x, y) };
            var pointInReducedBounds = safeBounds((x, y));
            foreach (var p in news)
            {
                if ((pointInReducedBounds || flowMap.inBounds(p.x, p.y)) && !seenMap[p.x][p.y])
                {
                    candidates[currentIdx++] = (p.x, p.y);
                    seenMap[p.x][p.y] = true;
                }

            };
        };
        Array.Resize(ref candidates, currentIdx);
        return candidates;
    }

    private static IFlowMap.PointFlow[] collectChanged((int x, int y)[] candidates, SimpleFlowMap flowMap, IHeightMap heightMap)
    {
        var changedCandidates = new IFlowMap.PointFlow[candidates.Length];
        int changedIdx = 0;
        for (int i = 0; i < candidates.Length; i++)
        {
            var (x, y) = candidates[i];
            var f = flowMap.GetFlow(x, y);
            var (changed, newFlow) = PointFollowNeighbours(f, flowMap, heightMap);
            if (changed)
            {
                changedCandidates[changedIdx++] = new IFlowMap.PointFlow(x, y, newFlow);
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
    private static (bool changed, SimpleFlowMap flowMap, IFlowMap.PointFlow[] changedPoints) ExpandExistingFlow(SimpleFlowMap flowMap, IHeightMap heightMap, IFlowMap.PointFlow[] previousChanged, bool[][] seen)
    {
        var candidates = collectNeighbours(previousChanged, flowMap, seen);
        var changedCandidates = collectChanged(candidates, flowMap, heightMap);
        bool changeOccured = changedCandidates.Length != 0;

        //apply all changes to flowmap
        if (changeOccured)
        {
            foreach (var flowPoint in changedCandidates)
            {
                flowMap.SetFlow(flowPoint.X, flowPoint.Y, flowPoint.Flow);
            }
        }

        return (changeOccured, flowMap, changedCandidates);
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
        Console.WriteLine("calculate edge flow");
        ApplyNaturalEdgeFlow(heightMap, flowMap);
        Console.WriteLine("expand flow");

        //get all existing flows
        var edges = new List<IFlowMap.PointFlow>();
        foreach (var point in flowMap.GetPoints())
        {
            if (!point.Flow.Unknown)
                edges.Add(point);
        }

        var changed = true;
        var i = 0;
        var seenMap = new bool[flowMap.getDimensions().x][];
        for (int x = 0; x < flowMap.getDimensions().x; x++)
        {
            seenMap[x] = new bool[flowMap.getDimensions().y];
        }
        
        //first candidates are the natural edges
        var previousCandidated = edges.ToArray();
        foreach( var candidated in previousCandidated)
            seenMap[candidated.X][candidated.Y] = true;

        while (changed)
        {
            //EntryClass.SaveToFile($"iteration {i}:\n" + FlowMapPrinter.FlowMapToString(flowMap, heightMap), true);
            var expanded = ExpandExistingFlow(flowMap, heightMap, previousCandidated, seenMap);
            flowMap.flowMap = expanded.flowMap.flowMap;
            changed = expanded.changed;
            previousCandidated = expanded.changedPoints;
            Console.WriteLine($"expanding flowmap at iteration: {i}");
            i++;
        }
    

        Console.WriteLine("done");
    }

    public static SKBitmap ToImage(IFlowMap flowMap)
    {
        var (width, height) = flowMap.getDimensions();
        byte[] pixelData = new byte[width * height];

        foreach (var points in flowMap.GetPoints())
        {
            pixelData[points.Y * width + points.X] = FlowTranslation.FlowToGray8(points.Flow);
        }

        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Gray8, SKAlphaType.Opaque));
        IntPtr pixelsPointer = Marshal.UnsafeAddrOfPinnedArrayElement(pixelData, 0);
        bitmap.SetPixels(pixelsPointer);
        return bitmap;
    }

    public static SKBitmap ToColorImage(IFlowMap flowMap)
    {
        var (width, height) = flowMap.getDimensions();
        uint[] pixelData = new uint[width * height];

        foreach (var points in flowMap.GetPoints())
        {
            pixelData[points.Y * width + points.X] = FlowTranslation.FlowToColor(points.Flow);
        }

        SKBitmap bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque));
        IntPtr pixelsPointer = Marshal.UnsafeAddrOfPinnedArrayElement(pixelData, 0);
        bitmap.SetPixels(pixelsPointer);
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

    public List<IFlowMap.PointFlow> FollowFlow(IFlowMap.PointFlow point)
    {
        List<IFlowMap.PointFlow> nextFlow = new();
        if (point.Flow.Up)
            nextFlow.Add(GetFlow(Point.Up(point.X, point.Y).x, Point.Up(point.X, point.Y).y));
        if (point.Flow.Down)
            nextFlow.Add(GetFlow(Point.Down(point.X, point.Y).x, Point.Down(point.X, point.Y).y));
        if (point.Flow.Left)
            nextFlow.Add(GetFlow(Point.Left(point.X, point.Y).x, Point.Left(point.X, point.Y).y));
        if (point.Flow.Right)
            nextFlow.Add(GetFlow(Point.Right(point.X, point.Y).x, Point.Right(point.X, point.Y).y));

        return nextFlow;
    }

    public (int x, int y) getDimensions()
    {
        return (flowMap.Length, flowMap[0].Length);
    }

    public IFlowMap.PointFlow GetFlow(int x, int y)
    {
        return new IFlowMap.PointFlow(x, y, flowMap[x][y]);
    }

    public IEnumerable<IFlowMap.PointFlow> GetPoints()
    {
        for (int y = 0; y < getDimensions().y; y++)
        {
            for (int x = 0; x < getDimensions().x; x++)
            {
                yield return new IFlowMap.PointFlow(x, y, flowMap[x][y]);
            }
        }
    }

    public void SetFlow(int x, int y, IFlowMap.Flow flow)
    {
        flowMap[x][y] = flow;
    }

    public IEnumerable<IFlowMap.PointFlow> GetRow(int y)
    {
        for (int x = 0; x < getDimensions().x; x++)
        {
            yield return GetFlow(x, y);
        }
    }

    public IEnumerable<IEnumerable<IFlowMap.PointFlow>> PointsByLine()
    {
        for (int y = getDimensions().y - 1; y >= 0; y--)
        {
            yield return GetRow(y);
        }
    }

    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < getDimensions().x && y >= 0 && y < getDimensions().y; ;
    }
}
