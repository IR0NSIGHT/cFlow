using cFlowForms;
using SkiaSharp.Views.Desktop;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        private float scale = 1;
        private float ratio = 1;
        private (int x, int y) position = (0, 0);
        private MoveMapByMouse mouseMover;
        private cFlowApi.CFlowGenerator cFlowApi;
        public MainWindow()
        {
            InitializeComponent();
            riverSpacingNumericChanged(null, null);

            mouseMover = new MoveMapByMouse(heightPictureBox, SetCenter, GetCenter); ;
            MouseDown += (p, p1) =>
            {
                Console.WriteLine(p1);
            };

            heightPictureBox.Paint += PictureBox1_Paint;
            flowPicturBox.Paint += PictureBox1_Paint;
            riverPictureBox.Paint += PictureBox1_Paint;
            this.MouseWheel += MainForm_MouseWheel;
            this.KeyDown += Form1_KeyDown;

            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;
            genManyRiverButton.Click += OnGenerateRiverButton;
            heightPictureBox.Click += handleSpawnRiverMouseClick;

            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats.png";
            cFlowApi = new cFlowApi.CFlowGenerator(path + file);
            ratio = cFlowApi.HeightmapImg.Width / cFlowApi.HeightmapImg.Height;
            heightPictureBox.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(cFlowApi.HeightmapImg);


        }

        private void OnGenerateRiverButton(object? sender, EventArgs e)
        {
            cFlowApi.SpamRivers(riverSpacing.x, riverSpacing.y);
            riverPictureBox.Image = SkiaSharp.Views.Desktop.Extensions.ToBitmap(cFlowApi.RivermapImg);

            RedrawMaps();
        }

        public System.Drawing.Point GetCenter() { return new System.Drawing.Point(position.x, position.y); }
        public void SetCenter(System.Drawing.Point center)
        {
            position = (center.X, center.Y);
            RedrawMaps();
        }

        private void RedrawMaps()
        {
            heightPictureBox.Invalidate();
            flowPicturBox.Invalidate();
            riverPictureBox.Invalidate();
        }

        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            scale += (e.Delta * 1f / 1000);
            labelCurrentScale.Text = scale.ToString();
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
                    new Rectangle(position.x, position.y, (int)(pictureBox.Width / scale), (int)((pictureBox.Width * ratio) / scale)),
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

        private void onGenerateFlowButton(object sender, EventArgs e)
        {
            cFlowApi.GenerateFlow();
            flowPicturBox.Image = cFlowApi.FlowmapImgColored.ToBitmap();


            RedrawMaps();
        }

        private void handleSpawnRiverMouseClick(object? sender, EventArgs e)
        {
            if (e is not MouseEventArgs { Button: MouseButtons.Left } mouseArgs)
                return;
            if (!spawnRiverMode)
                return;
            (int x, int y) deltaPixel = ((int)(mouseArgs.X), (int)(mouseArgs.Y )); //(mouseArgs.X - heightPictureBox.Location.X, mouseArgs.Y - heightPictureBox.Location.Y);
            (int x, int y)  deltaPos = ((int)(deltaPixel.x / scale), (int)(deltaPixel.y / scale));
            var clickedMapPos = (GetCenter().X + deltaPos.x, GetCenter().Y + deltaPos.y);
            cFlowApi.RiverMap.AddRiverFrom(clickedMapPos);
            riverPictureBox.Image = cFlowApi.RiverMap.ToImage().ToBitmap();
            RedrawMaps();
        }


        private (int x, int y) riverSpacing = (10, 10);

        private void riverSpacingNumericChanged(object? sender, EventArgs e)
        {
            riverSpacing = (decimal.ToInt32(numericRiverSpacingX.Value), decimal.ToInt32(numericRiverSpacingY.Value));
        }

        private bool spawnRiverMode = false;
        private void OnSpawnRiverButtonClick(object sender, EventArgs e)
        {
            spawnRiverMode = !spawnRiverMode;
            spawnSingleRiverButton.BackColor = spawnRiverMode ? Color.DeepSkyBlue : Color.LightGray;
            spawnSingleRiverButton.Text = spawnRiverMode ? "Spawn single river: Active" : "Spawn single river";
        }
    }
}
