﻿using SkiaSharp;
using src.Maps.riverMap;
using System.Diagnostics;
using System.Text;

public class EntryClass
{
    static void Main(string[] args)
    {
        var filename = "C:\\Users\\Max1M\\OneDrive\\Bilder\\tiny_heightmap.png";
        var fileOut = "C:\\Users\\Max1M\\OneDrive\\Bilder\\tiny_heightmap_EDIT.png";

        SKBitmap sKBitmap = ImageApi.LoadBitmapFromPng(filename);
        Console.WriteLine("Loaded bitmap");
        IHeightMap heightMap = new Image8BitHeightMap(sKBitmap);
        SimpleFlowMap fMap = new SimpleFlowMap(heightMap.Bounds()).FromHeightMap(heightMap);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var coloredFlow = SimpleFlowMap.ToColorImage(fMap);
        ImageApi.SaveBitmapAsPng(coloredFlow, fileOut);

        SaveToFile(FlowMapPrinter.FlowMapToString(fMap, heightMap), false);

        stopwatch.Stop();
        TimeSpan elapsed = stopwatch.Elapsed;
        stopwatch.Reset();

        var riverMap = new RiverMap(fMap);
        riverMap.AddRiverFrom(69, 25);

        Console.WriteLine($"Seconds: {elapsed.TotalSeconds}");

        Console.WriteLine($"saved flowmap to {fileOut}");

    }

    public static void SaveToFile(string content, bool append)
    {
        // Set a variable to the Documents path.
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Open or create the file and write the content
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "cFlow_Log.txt"), append, Encoding.UTF8 ))
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
