using System.Security;
using System.Windows;
using System.Windows.Input;
using cFlowForms;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            SetupChannels();
            MapView.SetChannels(this._guiEventChannel, this._backendEventChannel);
            LoadDefaultMap();


            this.ImportHeightmap.OnToggledEventHandler += OnImportHeightmapButtonClick;
            this.CalcFlowButton.OnToggledEventHandler += OnCalcFlowButtonClick;

            var riverTool = new RiverTool(_guiEventChannel);

            this.RiverToolButton.OnToggledEventHandler += riverTool.OnToggleToolClicked;
            this.MapView.OnMapClicked += riverTool.OnMapClicked;

            this.MapView.OnMapClicked += OnMapClicked;


            _backendEventChannel.LoadingStateChanged += OnLoadingUpdate;
        }

        private void OnMapClicked(object? sender, ((int x, int y) pos, MouseEventArgs e) arg)
        {
            if (arg.e.LeftButton == MouseButtonState.Pressed && this.FloodToolButton.isActive())
                _guiEventChannel.RequestFloodChange(
                    new GuiEvents.FloodChangeRequestEventArgs(
                        arg.pos, 
                        GuiEvents.FloodChangeType.Add,
                        10,
                        400000
                        
                        ));
        }

        private GuiEventChannel _guiEventChannel;
        private BackendEventChannel _backendEventChannel;

        public void SetupChannels()
        {
            var guiChannel = new GuiEventChannel();
            var backend = new Backend();
            var gui = this;
            if (gui == null || !(gui is MainWindow mainWindow))
            {
                throw new Exception("Main Window is null or wrong type on setup");
            }
            var backendChannel = new BackendEventChannel(mainWindow);
            mainWindow.Populate(guiChannel, backendChannel);
            backend.Populate(guiChannel, backendChannel);
        }

        private void LoadDefaultMap()
        {
            //load Heightmap
            var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            var file = "medium_flats_brokenUp.png";
            _guiEventChannel.RequestLoadHeightmap(new FileEventArgs(path + file));
        }

        public void Populate(GuiEventChannel guiEventChannel, BackendEventChannel backendEventChannel)
        {
            this._backendEventChannel = backendEventChannel;
            this._guiEventChannel = guiEventChannel;
        }

        private void OnCalcFlowButtonClick(object sender, bool newState)
        {
            _guiEventChannel.RequestCalculateFlow();
        }

        private void OnLoadingUpdate(object? sender, LoadingStateEventArgs e)
        {
            if (e.IsLoading)
                LoadingSpinner.Visibility = Visibility.Visible;
            else
                LoadingSpinner.Visibility = Visibility.Hidden;
        }

        private void OnImportHeightmapButtonClick(object sender, bool newState)
        {
            var openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Select a PNG file",
                Filter = "PNG files (*.png)|*.png",
                Title = "Open PNG file",
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    string selectedDirectory = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                    string fileName = System.IO.Path.GetFileName(openFileDialog1.FileName);

                    _guiEventChannel.RequestLoadHeightmap(new FileEventArgs(openFileDialog1.FileName));
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