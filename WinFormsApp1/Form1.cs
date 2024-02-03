using cFlowForms;
using SkiaSharp;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        private float scale = 1;
        private float ratio = 1;
        private (int x, int y) position = (0, 0);
        private MoveMapByMouse mouseMover;
        public MainWindow()
        {
            InitializeComponent();
            mouseMover = new MoveMapByMouse(pictureBox1, SetCenter, GetCenter); ;

            MouseDown += (p,p1) =>
            {
                Console.WriteLine(p1);
            };

            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox2.Paint += PictureBox1_Paint;
            pictureBox3.Paint += PictureBox1_Paint;
            this.MouseWheel += MainForm_MouseWheel;
            this.KeyDown += Form1_KeyDown;

            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            var gen = new cFlowApi.CFlowGenerator(path + file);
            ratio = gen.HeightmapImg.Width / gen.HeightmapImg.Height;
            pictureBox1.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.HeightmapImg);
            pictureBox2.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.FlowmapImgColored);
            pictureBox3.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(gen.RivermapImg);
        }

        public System.Drawing.Point GetCenter() { return new System.Drawing.Point(position.x, position.y); }
        public void SetCenter(System.Drawing.Point center)
        {
            position = (center.X, center.Y);
            RedrawMaps();
        }

        private void RedrawMaps()
        {
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
        }

        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            scale += (e.Delta * 1f / 1000);
            RedrawMaps();
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
                    new Rectangle(position.x, position.y, (int)(pictureBox.Image.Width / scale), (int)(pictureBox.Image.Height / scale)),
                    GraphicsUnit.Pixel);
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Check if both the left and right arrow keys are pressed simultaneously
            if (e.KeyCode == Keys.Left && (Control.ModifierKeys & Keys.Right) == Keys.Right)
            {
                MessageBox.Show("Left and Right arrow keys are pressed simultaneously");
            }

            Console.WriteLine("KeyData is: " + e.KeyData.ToString());
            int moveSpeed = 2;
            if (e.KeyCode == Keys.Down)
            {
                // Up arrow key is pressed
                position = (position.x, position.y + moveSpeed);
            }
            if (e.KeyCode == Keys.Up)
            {
                // Down arrow key is pressed
                position = (position.x, position.y - moveSpeed);
            }
            if (e.KeyCode == Keys.Left)
            {
                // Left arrow key is pressed
                position = (position.x - moveSpeed, position.y);
            }
            if (e.KeyCode == Keys.Right)
            {
                // Right arrow key is pressed
                position = (position.x + moveSpeed, position.y);
            }
            RedrawMaps();

        }

    }
}
