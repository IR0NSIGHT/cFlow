using cFlowForms;
using SkiaSharp.Views.Desktop;
using static cFlowForms.GuiEvents;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        public EventHandler? FlowCalculationRequestHandler;
        public EventHandler<RiverChangeRequestEventArgs>? RiverChangeRequestHandler;


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

            heightPictureBox.Paint += PictureBox1_Paint;
            flowPicturBox.Paint += PictureBox1_Paint;
            riverPictureBox.Paint += PictureBox1_Paint;
            this.MouseWheel += MainForm_MouseWheel;

            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;
            
            genManyRiverButton.Click += OnGenerateMassRiverButton;
            heightPictureBox.Click += handleSpawnRiverMouseClick;
        }


        public void OnMessageRaised(object? sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, e.Type.ToString());
        }

        public void OnLoadingStateChanged(object? sender, LoadingStateEventArgs e)
        {
            //TODO loadingspinner
        }

        public void OnHeightmapChanged(object? sender, ImageEventArgs e)
        {
            heightPictureBox.Image = e.Image;
            ratio = e.Image.Height * 1f / e.Image.Width;
            heightPictureBox.Invalidate();
        }

        public void OnFlowmapChanged(object? sender, ImageEventArgs e)
        {
            flowPicturBox.Image = e.Image;
            flowPicturBox.Invalidate();
        }

        public void OnRivermapChanged(object? sender, ImageEventArgs e)
        {
            riverPictureBox.Image = e.Image;
            riverPictureBox.Invalidate();
        }


        private void OnGenerateMassRiverButton(object? sender, EventArgs e)
        {
            //TODO use events
            MessageBox.Show("not implemented");
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

        private void onGenerateFlowButton(object sender, EventArgs e)
        {
            //TODO use events
            FlowCalculationRequestHandler?.Invoke(this, EventArgs.Empty);
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
            //TODO use events
            RiverChangeRequestHandler?.Invoke(this,
                new RiverChangeRequestEventArgs(clickedMapPos, RiverChangeType.Add));
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
