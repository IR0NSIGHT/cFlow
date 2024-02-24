using System.Diagnostics;
using System.Drawing;
using src.Maps.riverMap;
using System.Text;
using application.Maps.flowMap;
using cFlowApi.Heightmap;

namespace cFlowApi
{
    public class CFlowGenerator
    {
        private Bitmap _flowmapImgColored;
        private Bitmap _rivermapImg;

        public DummyDimension Heightmap { get; }
        private DistanceMap _flowMap;
        public Bitmap FlowmapImgColored { get => _flowmapImgColored; }
        public Bitmap RivermapImg { get => _rivermapImg; }

        public RiverMap RiverMap;

        public CFlowGenerator(string imagePath)
        {
            Debug.WriteLine("Start reading image");

            Heightmap = DummyDimension.ImportFromFile(imagePath);

            Debug.WriteLine("Converted image to inputDistanceMap");


            _flowMap = new DistanceMap(heightMap: Heightmap);
            Debug.WriteLine("loaded distanceMap");

            RiverMap = new RiverMap(_flowMap, Heightmap);
            Debug.WriteLine("loaded rivermap");


        }

        public void GenerateFlow()
        {
            var sw = Stopwatch.StartNew();
            _flowMap.CalculateFromHeightmap();
            Debug.WriteLine($"calculating distance map for {_flowMap.Bounds().x/ 1000}x{_flowMap.Bounds().y/ 1000} image took {sw.ElapsedMilliseconds} millis.");
            sw.Restart();
            _flowmapImgColored = _flowMap.toImage();
            Debug.WriteLine($"distance map´toImage for {_flowMap.Bounds().x/1000}x{_flowMap.Bounds().y/1000} image took {sw.ElapsedMilliseconds} millis.");
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
