using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for TierCard.xaml
    /// </summary>
    public partial class TierCard : UserControl
    {
        public TierCard()
        {
            InitializeComponent();

            originalImage = new BitmapImage(new Uri("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\textureGrid_1k.jpg", UriKind.Absolute));
            _currentMapSection = new MapSection(0, 0, originalImage.PixelWidth);
            this.Loaded += RedrawMap;
        }

        private BitmapImage originalImage;

        record struct MapSection
        {
            public int PosX;
            public int PosY;
            public int DisplayWidth;

            public MapSection(int posX, int posY, int displayWidth) =>
                (PosX, PosY, DisplayWidth) = (posX, posY, displayWidth);
        }

        private MapSection _currentMapSection = new();
        private Int32Rect getDisplayMapCrop(double guiWidth, double guiHeight) => new Int32Rect(
            this._currentMapSection.PosX,
            this._currentMapSection.PosY,
            this._currentMapSection.DisplayWidth,
            (int)(_currentMapSection.DisplayWidth * (guiHeight / (guiWidth)))
        );

        private void RedrawMap(object sender, RoutedEventArgs e)
        {
            // Load the original image

            var section = getDisplayMapCrop(this.ActualWidth, this.ActualHeight);
            CroppedBitmap cropped_bitmap =
                new CroppedBitmap(originalImage, section);

            MapImage.Source = cropped_bitmap;
        }

        private float currentScale = 1;
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            PreviewMouseWheel(this, e);
        }

        private void SetSectionByScale(float scale)
        {
            _currentMapSection = _currentMapSection with { DisplayWidth = (int)(originalImage.PixelWidth / scale) };
        }

        private void updateScale(float delta)
        {
            this.currentScale = Math.Clamp(currentScale+delta, 1, 5);
            
        }
        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            updateScale(e.Delta * 1f / 1000);
            Debug.WriteLine($"scale = {currentScale}");
            SetSectionByScale(currentScale);
            RedrawMap(sender, e);
        }
    }
}
