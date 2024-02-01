using SkiaSharp;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        private float scale = 1;
        private float ratio = 1;

        public MainWindow()
        {
            InitializeComponent();
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            var gen = new cFlowApi.CFlowGenerator(path + file);
            ratio = gen.HeightmapImg.Width / gen.HeightmapImg.Height;
            pictureBox1.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.HeightmapImg);
            pictureBox2.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.FlowmapImgColored);
            pictureBox3.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.RivermapImg);

            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox2.Paint += PictureBox1_Paint;
            pictureBox3.Paint += PictureBox1_Paint;
            this.MouseWheel += MainForm_MouseWheel;
        }

        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            scale += (e.Delta * 1f / 1000);
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
        }

        private void PictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Image != null)
            {

                e.Graphics.FillRectangle(new SolidBrush(Color.Orange), 0, 0, pictureBox.Width, pictureBox.Height);

                // Draw the portion of the image within the calculated rectangle
                e.Graphics.DrawImage(
                    pictureBox.Image,
                    new Rectangle(0, 0, pictureBox.Width, (int)(pictureBox.Width * ratio)),
                    new Rectangle(0, 0, (int)(pictureBox.Image.Width / scale), (int)(pictureBox.Image.Height / scale )),
                    GraphicsUnit.Pixel);
            }
        }
    }
}
