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
            this._mapPositioner = new MapPositioner(this);
            _mapPositioner.OnMapSectionChanged += RedrawMap;

            _layerProvider = new LayerProvider(this.ButtonList);
            _layerProvider.LayerToggledEventHandler += OnLayerToggled;
            this.MouseDown += OnMouseDown;
        }

        private void OnLayerToggled(object? sender, EventArgs e)
        {
            RedrawMap();
        }

        private void RedrawMap()
        {
            RedrawMap(this, _mapPositioner.getDisplayedAreaOfMapImage(this.ActualWidth, this.ActualHeight));
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
            if (this.ActualHeight == 0 || this.ActualWidth == 0)
                return;

            ScaleText.Text = $"{displayedSection.Width / 3} blocks";

            Debug.Assert(displayedSection.HasArea, " displayedSection is illegal shape");
            ImageOverlay.Children.Clear();
            foreach (var bitmap in _layerProvider.ActiveLayers())
            {
                IntPtr hBitmap = bitmap.GetHbitmap();
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
                );
                Image img = new Image();
                img.Source = bitmapSource;
                ImageOverlay.Children.Add(img);
            }
        /*    try
            {
                CroppedBitmap cropped_bitmap =
                    new CroppedBitmap(_heightBitmapSource, displayedSection);

                MapImage.Source = cropped_bitmap;
                InfoText.Text = $"{_heightBitmapSource.PixelWidth} x {_heightBitmapSource.PixelHeight} blocks";

            }
            catch (Exception ex)
            {
                Debug.WriteLine("UWU");
            } */
        }
    }
}
