using System.Drawing.Drawing2D;
using System.Security;
using cFlowForms;
using static cFlowForms.GuiEvents;
using Button = System.Windows.Forms.Button;
using Rectangle = System.Drawing.Rectangle;

namespace WinFormsApp1
{

    public partial class MainWindow : Form
    {
        private float scale = 1;
        private float ratio = 1;
        private (int x, int y) position = (0, 0);
        private GuiEventChannel channel;
        private LayerProvider layerProvider;
        public MainWindow()
        {
            InitializeComponent();

            layerProvider = new LayerProvider();
            layerProvider.LayerToggledEventHandler += OnLayerToggled;

            new MoveMapByMouse(heightPictureBox, SetCenter, GetCenter); ;


            //connect own buttons and controls
            heightPictureBox.Paint += MapPictureBox_Paint;
            this.MouseWheel += MainForm_MouseWheel;

            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;

            riverSplitProbNumeric.ValueChanged += OnRiverSplitProbNumericChanged;

            maxLakeDepthNumeric.ValueChanged += maxLakeDepthNumericChanged;
            maxLakeSurfaceNumeric.ValueChanged += maxLakeSurfaceNumericChanged;

            genManyRiverButton.Click += OnGenerateMassRiverButton;
            heightPictureBox.Click += handleMouseClickOnMap;

            BuildLayerToggleButtons();
        }

        private int riverSplitProbability = 10;
        private void OnRiverSplitProbNumericChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown n)
            {
                this.riverSplitProbability = decimal.ToInt32(n.Value);
            }
        }

        private void OnLayerToggled(object? sender, EventArgs e)
        {
            RedrawMaps();
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
            if (e.IsLoading)
            {
                loading_spinner_box.Visible = true;
            }
            else
            {
                loading_spinner_box.Visible = false;
            }

        }

        private void BuildLayerToggleButtons()
        {

            while (LayerTogglePanel.Controls.Count > 0)
            {
                var button = LayerTogglePanel.Controls[0];
                if (button is Button)
                {
                    button.Click -= OnLayerToggleButtonClick;
                }
                button.Dispose();
            }

            // Sample list of strings (you can replace this with your dynamic list)
            List<string> stringList = new List<string> { "Button 1", "Button 2", "Button 3" };

            // Create buttons dynamically for each string in the list
            foreach (var x in layerProvider.AllLayers())
            {
                Button newButton = new Button();
                newButton.Text = x.name;
                newButton.Name = x.name;
                newButton.Size = new System.Drawing.Size(100, 30);
                newButton.BackColor = x.active ? StyledButton.HighlightColor : StyledButton.BackColor;
                newButton.Click += OnLayerToggleButtonClick;
                LayerTogglePanel.Controls.Add(newButton);
            }
        }

        private void OnLayerToggleButtonClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                int idx = -1;
                foreach (var xLayer in layerProvider.AllLayers())
                {
                    if (button.Name == xLayer.name)
                    {
                        idx = xLayer.idx;
                        break;
                    }
                }

                if (idx == -1)
                    return;
                layerProvider.ToggleLayer(idx);
                button.BackColor = layerProvider.IsLayerActive(idx) ? StyledButton.HighlightColor : StyledButton.BackColor;

            }
        }

        public void OnHeightmapChanged(object? sender, ImageEventArgs e)
        {
            if (e.MapType == MapType.Heightmap)
                layerProvider.UpdateLayerBitmap(LayerProvider.HeightmapLayer, e.Image);
            else if (e.MapType == MapType.ContourLines)
                layerProvider.UpdateLayerBitmap(LayerProvider.ContourLayer, e.Image);


            ratio = e.Image.Height * 1f / e.Image.Width;
            heightPictureBox.Invalidate();
            BuildLayerToggleButtons();
        }

        public void OnFlowmapChanged(object? sender, ImageEventArgs e)
        {
            layerProvider.UpdateLayerBitmap(LayerProvider.FlowLayer, e.Image);
            heightPictureBox.Invalidate();
            BuildLayerToggleButtons();
        }

        public void OnRivermapChanged(object? sender, ImageEventArgs e)
        {
            layerProvider.UpdateLayerBitmap(LayerProvider.RiverLayer, e.Image);
            heightPictureBox.Invalidate();
            BuildLayerToggleButtons();
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
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                e.Graphics.FillRectangle(new SolidBrush(StyledButton.BackColor), 0, 0, pictureBox.Width, pictureBox.Height);

                foreach (var layerImg in layerProvider.ActiveLayers())
                {
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

        private void handleMouseClickOnMap(object? sender, EventArgs e)
        {
            if (e is not MouseEventArgs mouseArgs)
                return;

            (int x, int y) deltaPixel = ((int)(mouseArgs.X), (int)(mouseArgs.Y)); //(mouseArgs.X - heightPictureBox.Location.X, mouseArgs.Y - heightPictureBox.Location.Y);
            (int x, int y) deltaPos = ((int)(deltaPixel.x / scale), (int)(deltaPixel.y / scale));
            var clickedMapPos = (GetCenter().X + deltaPos.x, GetCenter().Y + deltaPos.y);
            handleSpawnRiverOnMap(clickedMapPos, this.riverToolActive);
            handleFloodAreaOnMap(clickedMapPos, this.floodToolActive);
        }

        private void handleSpawnRiverOnMap((int x, int y) mapPos, bool doUse)
        {
            if (!doUse)
                return;
            channel.RequestRiverChange(new RiverChangeRequestEventArgs(mapPos, RiverChangeType.Add) { splitEveryXBlocks = riverSplitProbability });
        }

        private void handleFloodAreaOnMap((int x, int y) mapPos, bool doUse)
        {
            if (!doUse)
                return;
            channel.RequestFloodChange(new FloodChangeRequestEventArgs(mapPos, FloodChangeType.Add) { MaxDepth = this.maxLakeDepth, MaxSurface = this.maxLakeSurface * this.maxLakeSurface });
        }

        private (int x, int y) riverSpacing = (10, 10);

        private void riverSpacingNumericChanged(object? sender, EventArgs e)
        {
            riverSpacing = (decimal.ToInt32(numericRiverSpacingX.Value), decimal.ToInt32(numericRiverSpacingY.Value));
        }

        private int maxLakeSurface = 100;
        private void maxLakeSurfaceNumericChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown numeric)
                maxLakeSurface = decimal.ToInt32(numeric.Value);
            maxLakeSurfaceLabel.Text = $"x {maxLakeSurface} m";
        }
        private int maxLakeDepth = 10;
        private void maxLakeDepthNumericChanged(object? sender, EventArgs e)
        {
            maxLakeDepth = decimal.ToInt32(numericRiverSpacingX.Value);
        }


        private bool riverToolActive = false;
        private void OnSpawnRiverButtonClick(object sender, EventArgs e)
        {
            riverToolActive = !riverToolActive;
            spawnSingleRiverButton.BackColor = riverToolActive ? StyledButton.HighlightColor : StyledButton.BackColor;
            spawnSingleRiverButton.Text = riverToolActive ? "Spawn single river: Active" : "Spawn single river";
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

        private bool floodToolActive = false;

        private void OnToggleFloodToolButtonClick(object sender, EventArgs e)
        {
            floodToolActive = !floodToolActive;
            if (sender is Button b)
            {
                b.BackColor = floodToolActive ? StyledButton.HighlightColor : StyledButton.BackColor;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
