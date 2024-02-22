using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using cFlowAPI.Maps.Shader;

namespace cFlowApi.Heightmap
{
    public class DummyDimension : IHeightMap
    {
        public static readonly int chunkSize = 10000;
        private Map2dIterator _iterator;
        private Bitmap _thumbNail;
        public DummyDimension((int x, int y) size, ushort height)
        {
            heightMap = filledHeightmap(size, height);
            _iterator = new Map2dIterator(size);
        }

        public static bool hasLowerNeighbours((int x, int y) point, IHeightMap heightMap)
        {
            var height = heightMap.GetHeight(point);
            var dim = (DummyDimension)heightMap;
            return dim.inBounds(Point.Up(point)) && heightMap.GetHeight(Point.Up(point)) < height ||
                   dim.inBounds(Point.Down(point)) && heightMap.GetHeight(Point.Down(point)) < height ||
                   dim.inBounds(Point.Left(point)) && heightMap.GetHeight(Point.Left(point)) < height ||
                   dim.inBounds(Point.Right(point)) && heightMap.GetHeight(Point.Right(point)) < height;
        }

        public static DummyDimension ImportFromFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var image = Image.FromStream(fs, false, false);
            var width = image.Width;
            var height = image.Height;

            var dim = new DummyDimension((width, height), 0);

            var dataArray = dim.heightMap;
            
            dim._thumbNail = thumbNail(image, Math.Min(image.Width, chunkSize), Math.Min(image.Height, chunkSize));
            for (int x = 0; x < width; x += chunkSize)
            {
                int yChunk = 0;
                for (int y = 0; y < height; y += chunkSize)
                {
                    var chunk = readChunk(image, x, y, Math.Min(width - x, chunkSize), Math.Min(height - y, chunkSize));
                    pixel16bitHeighmapArray(chunk, dataArray, x, y);
                }
            }
            return dim;
        }

        private static Bitmap readChunk(Image img, int startX, int startY, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            Rectangle sourceRect = new Rectangle(startX, startY, width, height);
            // Create a graphics object from the bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Use Graphics.DrawImage() to paint the original PNG image onto the bitmap
                graphics.DrawImage(img, new Rectangle(0, 0, width, height), sourceRect, GraphicsUnit.Pixel);

            }
            return bitmap;
        }

        public uint[,] ToGPUdata()
        {
            return convert(this.heightMap);
        }

        public void FromGPUdata(int[,] data)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    SetHeight((x, y), (ushort)data[x,y]);
                }
            }
        }

        private static Bitmap thumbNail(Image img, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            Rectangle sourceRect = new Rectangle(0, 0, img.Width, img.Height);
            // Create a graphics object from the bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Use Graphics.DrawImage() to paint the original PNG image onto the bitmap
                graphics.DrawImage(img, new Rectangle(0, 0, width, height), sourceRect, GraphicsUnit.Pixel);

            }
            return bitmap;
        }

        public (Bitmap, Bitmap) ShadedHeightmap()
        {
            //FIXME use actual to-scale data instead of thumbnail
            var sunlightMap = MapShaderApi.SunlightFromHeightmap(heightmap: _thumbNail);
            var contourMap = MapShaderApi.ContourFromHeightmap(heightmap: _thumbNail);
            return (sunlightMap, contourMap);
        }

        private static ushort[][] filledHeightmap((int x, int y) size, ushort height)
        {
            ushort[][] heightMap = arrayOfSize<ushort>(size.x, size.y);
            foreach (var row in heightMap)
            {
                Array.Fill(row, height);
            }
            return heightMap;
        }

        private ushort[][] heightMap = { [0, 0, 0], [0, 2, 3], [0, 1, 1] };
        public (int x, int y) Bounds()
        {
            int x = heightMap[0].Length;
            int y = heightMap.Length;
            return (x, y);
        }

        public ushort GetHeight((int x, int y) pos)
        {
            return heightMap[pos.y][pos.x];
        }

        public void SetHeight((int x, int y) pos, ushort z)
        {
            heightMap[pos.y][pos.x] = z;
        }

        public bool inBounds((int x, int y) point) =>
            inBounds(point.x, point.y);


        public bool inBounds(int x, int y) =>
            x >= 0 && x < Bounds().x && y >= 0 && y < Bounds().y;

        public IMapIterator<(int x, int y)> iterator()
        {
            return _iterator;
        }

        public static A[][] arrayOfSize<A>(int width, int height)
        {
            var rows = new A[height][];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new A[width];
            }

            return rows;
        }

        public static ushort[][] pixel16bitHeighmapArray(Bitmap bitmap)
        {
            var rows = arrayOfSize<ushort>(bitmap.Width, bitmap.Height);
            pixel16bitHeighmapArray(bitmap, rows, 0, 0);

            return rows;
        }

        public static void pixel16bitHeighmapArray(Bitmap bitmap, ushort[][] rows, int widthOffset, int heightOffset)
        {
            Debug.Assert(bitmap.Width * bitmap.Height < int.MaxValue);
            Debug.Assert(bitmap.PixelFormat == PixelFormat.Format32bppArgb);
            Debug.Assert(rows.Length >= heightOffset + bitmap.Height, "inputDistanceMap array is not tall enough to hold all data");
            Debug.Assert(rows[0].Length >= widthOffset + bitmap.Width, "inputDistanceMap array is not wide enough to hold all data");

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                IntPtr ptr = bmpData.Scan0;
                int totalItemsInData = bmpData.Stride * bmpData.Height;
                int[] rgbaRow = new int[bmpData.Stride / 4]; //bitmap always is loaded to uint32 rgba afaik (?)
                for (int i = 0; i < bitmap.Height; i++) //iterate over all rows
                {
                    int startIndex = i * bmpData.Stride;
                    Marshal.Copy(ptr + startIndex, rgbaRow, 0, rgbaRow.Length);

                    var heightDataRow = Array.ConvertAll(rgbaRow, x => (ushort)(byte)x);    //FIXME allow 16 bit images too, this one only reads last 8 bits
                    var targetArray = rows[i + heightOffset];
                    Array.Copy(heightDataRow, 0, targetArray, widthOffset, heightDataRow.Length);
                }
            }
            finally
            {
                // Unlock the bits.
                bitmap.UnlockBits(bmpData);
            }
        }

        private uint[,] convert(ushort[][] data)
        {
            uint[,] result = new uint[data.Length, data[0].Length];
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    result[y,x] = data[y][x];
                }
            }

            return result;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DummyDimension dim)
            {
                if (Bounds() != dim.Bounds())
                    return false;
                foreach (var point in iterator().Points())
                {
                    if (GetHeight(point) !=  dim.GetHeight(point))
                        return false;
                }
                return true;
            }

            return false;
        }
    }
}
