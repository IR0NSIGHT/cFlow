﻿using SkiaSharp;
using src.Maps.riverMap;
using System.Text;

namespace cFlowApi
{
    public class CFlowGenerator
    {
        private SKBitmap _heightmapImg;
        private SKBitmap _flowmapImgColored;
        private SKBitmap _rivermapImg;

        private IHeightMap _heightmap;
        private IFlowMap _flowMap;
        public SKBitmap HeightmapImg { get => _heightmapImg; }
        public SKBitmap FlowmapImgColored { get => _flowmapImgColored; }
        public SKBitmap RivermapImg { get => _rivermapImg; }

        public RiverMap RiverMap;

        public CFlowGenerator(string imagePath)
        {
            var _heightmapImg = ImageApi.LoadBitmapFromPng(imagePath);
            Console.WriteLine("Loaded bitmap");
            _heightmap = new Image8BitHeightMap(_heightmapImg);
            Console.WriteLine("Converted image to heightmap");

            this._heightmapImg = ((Image8BitHeightMap)_heightmap).ContourLinesOverlay();
            _flowMap = new SimpleFlowMap(_heightmap.Bounds());
            RiverMap = new RiverMap(_flowMap, _heightmap);
        }

        public void GenerateFlow()
        {
            SimpleFlowMap.CalculateFlowFromHeightMap(_heightmap, _flowMap);
            _flowmapImgColored = SimpleFlowMap.ToColorImage(_flowMap, FlowTranslation.FlowToColor);
        }

        public void SpamRivers(int xSpacing, int ySpacing)
        {
            Random r = new Random();
            for (int x = 0; x < RiverMap.Bounds().x; x += xSpacing)
            {
                for (int y = 0; y < RiverMap.Bounds().y; y += ySpacing)
                {
                    RiverMap.AddRiverFrom((x, y));

                }
            }
            _rivermapImg = RiverMap.ToImage();
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
    }
}
