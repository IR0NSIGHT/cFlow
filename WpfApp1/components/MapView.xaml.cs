using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            this._mapPositioner = new MapPositioner(this.MapCanvas);
            _mapPositioner.OnMapSectionChanged += RedrawMap;

            _layerProvider = new LayerProvider(this.ButtonList);
            _layerProvider.LayerToggledEventHandler += OnLayerToggled;
            MapCanvas.MouseDown += OnMouseDown;
            MapCanvas.PreviewMouseMove += OnPreviewMouseMove;
        }

        private void OnLayerToggled(object? sender, EventArgs e)
        {
            RedrawMap();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _mapPositioner.PreviewMouseWheel(sender, e);
            e.Handled = true;
        }

        private void RedrawMap()
        {
            var displayedSection = _mapPositioner.getDisplayedAreaOfMapImage();
            ScaleText.Text = $"{displayedSection.Width / 3} blocks";
            Debug.Assert(displayedSection.HasArea, " displayedSection is illegal shape");


            MapCanvas.Children.Clear();
            //set canvas to size of map times scale: scale = 2 ==> canvas is twice as big
            MapCanvas.Width = _mapPositioner.MapDimensions.width * _mapPositioner.CurrentScale;
            MapCanvas.Height = _mapPositioner.MapDimensions.height * _mapPositioner.CurrentScale;
            InfoText.Text = $"{displayedSection.Width} x {displayedSection.Height} blocks at x{_mapPositioner.CurrentScale} ";

            foreach (var imageSource in _layerProvider.ActiveLayers())
            {
                if ((imageSource.Width, imageSource.Height) != _mapPositioner.MapDimensions)
                    continue;
                
                var imageBrush = new ImageBrush(imageSource);
                imageBrush.Freeze();

                var rect = new System.Windows.Shapes.Rectangle
                {
                    Width = imageSource.Width * _mapPositioner.CurrentScale,
                    Height = imageSource.Height * _mapPositioner.CurrentScale,
                    Fill = imageBrush
                };

                MapCanvas.Children.Add(rect);
                Canvas.SetLeft(rect,displayedSection.X * _mapPositioner.CurrentScale);
                Canvas.SetTop(rect, displayedSection.Y * _mapPositioner.CurrentScale);

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
            var mouseMapPos = _mapPositioner.ToMapPxPos(e.GetPosition(this.MapCanvas));
            this.CurrentPosText.Text = $"mouse: {mouseMapPos.x}, {mouseMapPos.y}";
        }

        private void OnMouseDown(object? sender, MouseEventArgs args)
        {
            var mapPos = _mapPositioner.ToMapPxPos(args.GetPosition(this.MapCanvas));
            Debug.WriteLine($"user clicked map at mapcoord {mapPos} ");
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
