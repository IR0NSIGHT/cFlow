using System.Security;
using cFlowForms;
using static cFlowForms.GuiEvents;

namespace WinFormsApp1
{
    public partial class MainWindow : Form
    {
        private float scale = 1;
        private float ratio = 1;
        private (int x, int y) position = (0, 0);
        private GuiEventChannel channel;

        public MainWindow()
        {
            InitializeComponent();

            new MoveMapByMouse(heightPictureBox, SetCenter, GetCenter); ;


            //connect own buttons and controls
            heightPictureBox.Paint += MapPictureBox_Paint;
            this.MouseWheel += MainForm_MouseWheel;

            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;

            genManyRiverButton.Click += OnGenerateMassRiverButton;
            heightPictureBox.Click += handleSpawnRiverMouseClick;
        }

        public MainWindow Populate(GuiEventChannel guiEventChannel, BackendEventChannel backendChannel)
        {
            this.channel = guiEventChannel;

            //connect to backend callbacks
            backendChannel.FlowmapChanged += OnFlowmapChanged;
            backendChannel.HeightmapChanged += OnHeightmapChanged;
            backendChannel.RivermapChanged += OnRivermapChanged;
            backendChannel.MessageRaised += OnMessageRaised;
            backendChannel.LoadingStateChanged += OnLoadingStateChanged;

            return this;
        }

        public void OnMessageRaised(object? sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, e.Type.ToString());
        }

        public void OnLoadingStateChanged(object? sender, LoadingStateEventArgs e)
        {
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Value = e.LoadingProgress;

            if (e.IsLoading)
            {
                loading_spinner_box.Visible = true;
            }
            else
            {
                loading_spinner_box.Visible = false;
            }

        }

        private Bitmap? heightmap;
        private Bitmap? contours;
        private Bitmap? rivermap;
        private Bitmap? flowmap;

        public void OnHeightmapChanged(object? sender, ImageEventArgs e)
        {
            if (e.MapType == MapType.Heightmap)
            {
                heightmap = e.Image;
            }
            else if (e.MapType == MapType.ContourLines)
                contours = e.Image;

            ratio = e.Image.Height * 1f / e.Image.Width;
            heightPictureBox.Invalidate();
        }

        public void OnFlowmapChanged(object? sender, ImageEventArgs e)
        {
            flowmap = e.Image;
            heightPictureBox.Invalidate();
        }

        public void OnRivermapChanged(object? sender, ImageEventArgs e)
        {
            rivermap = e.Image;
            heightPictureBox.Invalidate();
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
        }

        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            scale += (e.Delta * 1f / 1000);
            labelCurrentScale.Text = scale.ToString();
            RedrawMaps();
        }

        private void MapPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Orange), 0, 0, pictureBox.Width, pictureBox.Height);

                foreach (var layerImg in new Bitmap?[] { this.heightmap, this.rivermap, this.contours })
                {
                    if (layerImg == null)
                        continue;
                    e.Graphics.DrawImage(
                        layerImg,
                        new Rectangle(0, 0, pictureBox.Width, (int)(pictureBox.Width * ratio)),
                        new Rectangle(position.x, position.y, (int)(pictureBox.Width / scale), (int)((pictureBox.Width * ratio) / scale)),
                        GraphicsUnit.Pixel);
                }
            }
        }

        private void onGenerateFlowButton(object sender, EventArgs e)
        {
            channel.RequestCalculateFlow();
        }

        private void handleSpawnRiverMouseClick(object? sender, EventArgs e)
        {
            if (e is not MouseEventArgs { Button: MouseButtons.Left } mouseArgs)
                return;
            if (!spawnRiverMode)
                return;
            (int x, int y) deltaPixel = ((int)(mouseArgs.X), (int)(mouseArgs.Y)); //(mouseArgs.X - heightPictureBox.Location.X, mouseArgs.Y - heightPictureBox.Location.Y);
            (int x, int y) deltaPos = ((int)(deltaPixel.x / scale), (int)(deltaPixel.y / scale));
            var clickedMapPos = (GetCenter().X + deltaPos.x, GetCenter().Y + deltaPos.y);

            channel.RequestRiverChange(new RiverChangeRequestEventArgs(clickedMapPos, RiverChangeType.Add));
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

        private struct ApplicationState
        {
            public string? lastFileDir;
            public string? currentHeightmapFile;
        }

        private ApplicationState state = new ApplicationState();
        private void OnImportHeightmapButtonClick(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Select a PNG file",
                Filter = "PNG files (*.png)|*.png",
                Title = "Open PNG file",
                //    RestoreDirectory = true
            };
            if (state.lastFileDir != null)
                openFileDialog1.InitialDirectory = state.lastFileDir;
            if (state.currentHeightmapFile != null)
                openFileDialog1.FileName = state.currentHeightmapFile;

            var diag = openFileDialog1.ShowDialog();
            if (diag == DialogResult.OK)
            {
                try
                {
                    string selectedDirectory = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                    string fileName = System.IO.Path.GetFileName(openFileDialog1.FileName);
                    if (selectedDirectory != null)
                    {
                        state.lastFileDir = selectedDirectory;
                    }

                    if (fileName != null)
                    {
                        state.currentHeightmapFile = fileName;
                    }

                    channel.RequestLoadHeightmap(new FileEventArgs(openFileDialog1.FileName));
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
    }
}
