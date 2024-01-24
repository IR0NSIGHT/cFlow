using SkiaSharp;
using src.Maps.riverMap;
using System.Diagnostics;
using System.Text;

public class EntryClass
{
    static void Main(string[] args)
    {
        var folder = "C:\\Users\\Max1M\\\\OneDrive\\Bilder\\";
        var filename = "generatedworld_heightmap";
        var fileOut = filename + "_EDIT";

        SKBitmap sKBitmap = ImageApi.LoadBitmapFromPng(folder + filename + ".png");
        Console.WriteLine("Loaded bitmap");
        IHeightMap heightMap = new Image8BitHeightMap(sKBitmap);
        Console.WriteLine("Converted image to heightmap");

        SimpleFlowMap fMap = new SimpleFlowMap(heightMap.Bounds());
        Console.WriteLine("Inited flowmap");

        SimpleFlowMap.CalculateFlowFromHeightMap(heightMap, fMap);
        Console.WriteLine("Calculateed Flow");

        var coloredFlow = SimpleFlowMap.ToColorImage(fMap, p => { if (p.Unknown) return new SKColor(255, 0, 0); else return new SKColor(0, 0, 0); });

        ImageApi.SaveBitmapAsPng(coloredFlow, folder + fileOut + ".png");
        List<IFlowMap.PointFlow> fuckedPoints = new List<IFlowMap.PointFlow>();
        foreach (var flow in fMap.GetPoints())
        {
            if (flow.Flow.Unknown)
            {
                fuckedPoints.Add(flow);
            }
        }
        Console.WriteLine($"saved flowmap to {fileOut}");

        //   SimpleFlowMap.CalculateFlowFromHeightMap(heightMap, fMap);

        //    var riverMap = new RiverMap(fMap);
        //   riverMap.AddRiverFrom(69, 25);


    }

    public static void SaveToFile(string content, bool append)
    {
        // Set a variable to the Documents path.
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Open or create the file and write the content
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "cFlow_Log.txt"), append, Encoding.UTF8))
        {
            outputFile.Write(content);
        }

        Console.WriteLine($"Content {(append ? "appended to" : "written to")} cFlow_Log.txt");
    }

    static void RandomizeHeight(IHeightMap d)
    {
        Random rand = new Random();
        foreach (var point in d.iterator().Points())
        {
            if (rand.NextDouble() < 0.01d)
                d.SetHeight(new Point(point.x, point.y, (short)(d.GetHeight(point.x, point.y) + rand.NextSingle() * 2)));
        }
    }
}
