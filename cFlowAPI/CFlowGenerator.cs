using SkiaSharp;
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

        public CFlowGenerator(string imagePath)
        {
            _heightmapImg = ImageApi.LoadBitmapFromPng(imagePath);
            Console.WriteLine("Loaded bitmap");
            _heightmap = new Image8BitHeightMap(_heightmapImg);
            Console.WriteLine("Converted image to heightmap");

           
        }

        public void GenerateFlow()
        {
            _flowMap = new SimpleFlowMap(_heightmap.Bounds());
            Console.WriteLine("Inited flowmap");

            SimpleFlowMap.CalculateFlowFromHeightMap(_heightmap, _flowMap);
            Console.WriteLine("Calculateed Flow");

            _flowmapImgColored = SimpleFlowMap.ToColorImage(_flowMap, FlowTranslation.FlowToColor);


        }

        public void SpamRivers(int xSpacing, int ySpacing)
        {
            var riverMap = new RiverMap(_flowMap);
            Random r = new Random();
            for (int x = 0; x < riverMap.Bounds().x; x+= xSpacing)
            {
                for (int y = 0; y < riverMap.Bounds().y; y+= ySpacing)
                {
                    riverMap.AddRiverFrom(x,y);

                }
            }
            _rivermapImg = riverMap.ToImage();
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
