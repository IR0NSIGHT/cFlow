using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using cFlowForms;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private BitmapSource _heightBitmapSource;
        public EventHandler<((int x, int y), MouseEventArgs)> OnMapClicked;
        private LayerProvider _layerProvider;
        public MapView()
        {
            InitializeComponent();
            this._mapPositioner = new MapPositioner(this.ImageOverlay);
            _mapPositioner.OnMapSectionChanged += RedrawMap;

            _layerProvider = new LayerProvider(this.ButtonList);
            _layerProvider.LayerToggledEventHandler += OnLayerToggled;
            this.MouseDown += OnMouseDown;
            this.ImageOverlay.PreviewMouseMove += OnPreviewMouseMove;
        }

        private void OnLayerToggled(object? sender, EventArgs e)
        {
            RedrawMap();
        }

        private void RedrawMap()
        {
            var displayedSection = _mapPositioner.getDisplayedAreaOfMapImage();

            if (this.ActualHeight == 0 || this.ActualWidth == 0)
                return;

            ScaleText.Text = $"{displayedSection.Width / 3} blocks";

            Debug.Assert(displayedSection.HasArea, " displayedSection is illegal shape");
            ImageOverlay.Children.Clear();
            foreach (var bitmap in _layerProvider.ActiveLayers())
            {
                if ((bitmap.Width, bitmap.Height) != _mapPositioner.MapDimensions)
                    continue;
                IntPtr hBitmap = bitmap.GetHbitmap();
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
                );
                Image img = new Image();

                CroppedBitmap cropped_bitmap =
                    new CroppedBitmap(bitmapSource, displayedSection);

                img.Source = cropped_bitmap;
                ImageOverlay.Children.Add(img);
                InfoText.Text = $"{displayedSection.Width} x {displayedSection.Height} blocks";

            }
        }

        private void OnFlowMapChanged(object? sender, ImageEventArgs e)
        {
            _layerProvider.UpdateLayerBitmap(LayerProvider.FlowLayer, e.Image);
            RedrawMap();
        }

        private void OnRiverMapChanged(object? sender, ImageEventArgs e)
        {
            _layerProvider.UpdateLayerBitmap(LayerProvider.RiverLayer, e.Image);
            RedrawMap();
        }

        private void OnHeightMapChanged(object? sender, ImageEventArgs args)
        {
            if (args.MapType != MapType.Heightmap)
                return;

            _layerProvider.UpdateLayerBitmap(LayerProvider.HeightmapLayer, args.Image);
            _mapPositioner.SetMapDimensions((args.Image.Width, args.Image.Height));
            RedrawMap();
        }

        private void OnPreviewMouseMove(object? sender, MouseEventArgs e)
        {
            var mouseMapPos = _mapPositioner.ToMapPxPos(e.GetPosition(this.ImageOverlay));
            this.CurrentPosText.Text = $"mouse: {mouseMapPos.x}, {mouseMapPos.y}";
        }

        private void OnMouseDown(object? sender, MouseEventArgs args)
        {
            var mapPos = _mapPositioner.ToMapPxPos(args.GetPosition(this));
            OnMapClicked.Invoke(this, (mapPos, args));
        }

        private MapPositioner _mapPositioner;

        public void SetChannels(GuiEventChannel guiChannel, BackendEventChannel backendChannel)
        {
            backendChannel.HeightmapChanged += OnHeightMapChanged;
            backendChannel.FlowmapChanged += OnFlowMapChanged;
            backendChannel.RivermapChanged += OnRiverMapChanged;
        }

        private void RedrawMap(object sender, Int32Rect displayedSection)
        {
            RedrawMap();
        }
    }
}
