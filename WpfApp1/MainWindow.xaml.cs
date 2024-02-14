using System.Windows;
using cFlowForms;

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
        }
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


            //load Heightmap
            //       var path = "C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\";
            //       var file = "medium_flats.png";
            //       backend.OnHeightmapPathSelected(null, new FileEventArgs(path + file));
        }

        private GuiEventChannel _guiEventChannel;
        private BackendEventChannel _backendEventChannel;
        public void Populate(GuiEventChannel guiEventChannel, BackendEventChannel backendEventChannel)
        {
            this._backendEventChannel = backendEventChannel;
            this._guiEventChannel = guiEventChannel;
        }
    }
}