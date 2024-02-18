using System.Diagnostics;
using System.Drawing;
using src.Maps.riverMap;
using System.Text;

namespace cFlowApi
{
    public class CFlowGenerator
    {
        private Bitmap _flowmapImgColored;
        private Bitmap _rivermapImg;

        public DummyDimension Heightmap { get; }
        private IFlowMap _flowMap;
        public Bitmap FlowmapImgColored { get => _flowmapImgColored; }
        public Bitmap RivermapImg { get => _rivermapImg; }

        public RiverMap RiverMap;

        public CFlowGenerator(string imagePath)
        {
            var _heightmapImg = ImageApi.LoadBitmapFromPng(imagePath);
            Debug.WriteLine("Loaded bitmap");
            Heightmap = new DummyDimension((30000, 30000),64);
            Debug.WriteLine("Converted image to heightmap");

            _flowMap = new SimpleFlowMap(Heightmap.Bounds());
            Debug.WriteLine("loaded flowmap");
            RiverMap = new RiverMap(_flowMap, Heightmap);
            Debug.WriteLine("loaded rivermap");

        }

        public void GenerateFlow()
        {
            SimpleFlowMap.CalculateFlowFromHeightMap(Heightmap, _flowMap);
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
