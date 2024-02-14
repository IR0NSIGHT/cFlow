using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using cFlowForms;
using static System.Windows.Point;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private BitmapSource _heightBitmapSource;
        public MapView()
        {
            InitializeComponent();
            this._mapPositioner = new MapPositioner(this);
            _mapPositioner.OnMapSectionChanged += RedrawMap;
        }

        private MapPositioner _mapPositioner;

        public void SetChannels(GuiEventChannel guiChannel, BackendEventChannel backendChannel)
        {
            backendChannel.HeightmapChanged += OnHeightMapChanged;
        }

        private void OnHeightMapChanged(object? sender, ImageEventArgs args)
        {
            if (args.MapType != MapType.Heightmap)
                return;

            IntPtr hBitmap = args.Image.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
            );

            SetMapImage(bitmapSource);
        }



        private void RedrawMap(object sender, Int32Rect displayedSection)
        {
            if (_heightBitmapSource == null || this.ActualHeight == 0 || this.ActualWidth == 0)
                return;

            ScaleText.Text = $"{displayedSection.Width / 3} blocks";

            Debug.Assert(displayedSection.HasArea, " displayedSection is illegal shape");
            try
            {
                CroppedBitmap cropped_bitmap =
                    new CroppedBitmap(_heightBitmapSource, displayedSection);

                MapImage.Source = cropped_bitmap;
                InfoText.Text = $"{_heightBitmapSource.PixelWidth} x {_heightBitmapSource.PixelHeight} blocks";

            }
            catch (Exception ex)
            {
                Debug.WriteLine("UWU");
            }
        }

        private void SetMapImage(BitmapSource newMap)
        {
            _mapPositioner.SetMapDimensions((newMap.PixelWidth, newMap.PixelHeight));
            this._heightBitmapSource = newMap;
            RedrawMap(this, _mapPositioner.getDisplayedAreaOfMapImage(this.ActualWidth, this.ActualHeight));
        }


    }
}
