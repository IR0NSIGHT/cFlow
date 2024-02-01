using SkiaSharp;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        private (int x, int y) currentCenter;
        private float scale = 1;
        private bool isDragging;
        System.Drawing.Point originalMousePos;

        private void SetCurrentCenter((int x, int y) currentCenter)
        {
            this.currentCenter = currentCenter;
        }

        public MainWindow()
        {
            InitializeComponent();
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            var gen = new cFlowApi.CFlowGenerator(path + file);
            //    pictureBox1.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.HeightmapImg);
            ShowMyImage(pictureBox1, SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.HeightmapImg));
            ShowMyImage(pictureBox2, SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.FlowmapImgColored));
            ShowMyImage(pictureBox3, SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.RivermapImg));

            pictureBox2.Paint += PictureBox1_Paint;
            this.MouseWheel += MainForm_MouseWheel;
        }

        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            // Handle the mouse wheel event here
            int delta = e.Delta; // Delta represents the amount the wheel was rotated
                                 // Perform actions based on the delta value, for example, zooming or scrolling
            scale += (delta*1f/1000);
            pictureBox2.Invalidate(); // Force PictureBox to repaint
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                originalMousePos = e.Location;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - originalMousePos.X;
                int deltaY = e.Y - originalMousePos.Y;
                if (Math.Abs(deltaX) < 5 && Math.Abs(deltaY) < 5)
                    return;

                SetCurrentCenter((currentCenter.x + deltaX, currentCenter.y + deltaY));
            }
        }

        public static void ShowMyImage(PictureBox box, Bitmap skBitmap)
        {
            var (xSize, ySize) = (box.Width, box.Height);

            // Calculate the new size while maintaining the aspect ratio
            float aspectRatio = (float)skBitmap.Width / skBitmap.Height;
            int newHeight;
            int newWidth;
            if (skBitmap.Width > skBitmap.Height)
            {
                //more wide than high
                newWidth = xSize;
                newHeight = (int)(xSize / aspectRatio);

            }
            else
            {
                //more high than wide
                newHeight = ySize;
                newWidth = (int)(ySize / aspectRatio);
            }


            //center image that might be smaller than box
            box.Location = CenterPosResizedImage(box.Location, (xSize, ySize), (newWidth, newHeight));

            // Stretches the image to fit the pictureBox.
            box.SizeMode = PictureBoxSizeMode.StretchImage;

            box.ClientSize = new Size(newWidth, newHeight);

            box.Image = (Image)skBitmap;

        }

        private void PictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                // Calculate the portion of the image to display
                Rectangle displayRect = new Rectangle(
                   0, 0,
                    pictureBox2.Width,
                    pictureBox2.Height);

                using (Brush orangeBrush = new SolidBrush(Color.Orange))
                {
                    e.Graphics.FillRectangle(orangeBrush, 0, 0, pictureBox2.Width, pictureBox2.Height);
                }

                // Draw the portion of the image within the calculated rectangle
                e.Graphics.DrawImage(
                    pictureBox2.Image,
                    displayRect,
                    new Rectangle(0, 0, (int)(pictureBox2.Image.Width / scale), (int)(pictureBox2.Image.Height / scale)),
                    GraphicsUnit.Pixel);


            }
        }


        /// <summary>
        /// image has oldPos with oldSize. redraw image to another size, have to recalc position so the new image is centered to the old images center
        /// built for resizing to smaller images.
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="oldSize"></param>
        /// <param name="newSize"></param>
        /// <returns>new position of image when using new size</returns>
        private static System.Drawing.Point CenterPosResizedImage(System.Drawing.Point oldPos, (int x, int y) oldSize, (int x, int y) newSize)
        {
            //center image that might be smaller than box
            int xPosition = (oldSize.x - newSize.x) / 2;
            int yPosition = (oldSize.y - newSize.y) / 2;
            return new System.Drawing.Point(oldPos.X + xPosition, oldPos.Y + yPosition);
        }
    }
}
