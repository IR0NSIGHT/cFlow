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
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();

            originalImage = new BitmapImage(new Uri("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\textureGrid_1k.jpg", UriKind.Absolute));
            _currentMapSection = new MapSection(0, 0, originalImage.PixelWidth);
            this.Loaded += RedrawMap;
            this.MouseMove += OnPreviewMouseMove;
            this.MouseRightButtonDown += OnPreviewMouseRightButtonDown;
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
        private Int32Rect getDisplayMapCrop(double guiWidth, double guiHeight)
        {
            int xWidth = _currentMapSection.DisplayWidth;
            int yHeight = (int)(_currentMapSection.DisplayWidth * (guiHeight / (guiWidth)));
            int xPos = (int)Math.Clamp(_currentMapSection.PosX, 0, originalImage.PixelWidth - xWidth);
            int yPos = (int)Math.Clamp(_currentMapSection.PosY, 0, originalImage.PixelWidth - yHeight);
            return new Int32Rect(
                xPos,
                yPos,
                xWidth,
                yHeight
            );
        }

        private void RedrawMap(object sender, RoutedEventArgs e)
        {
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
            this.currentScale = Math.Clamp(currentScale + delta, 1, 1000);
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            updateScale(e.Delta * 1f / 1000);
            Debug.WriteLine($"scale = {currentScale}");
            SetSectionByScale(currentScale);
            RedrawMap(sender, e);
        }

        private void UpdatePosition(int deltaX, int deltaY)
        {
            SetPosition(_currentMapSection.PosX + deltaX, _currentMapSection.PosY + deltaY);
        }

        private void SetPosition(int x, int y)
        {
            _currentMapSection = _currentMapSection with
            {
                PosX = Math.Clamp(x, 0, originalImage.PixelWidth),
                PosY = Math.Clamp(y, 0, originalImage.PixelHeight)
            };
        }

        private (int x, int y) dragStartPxPos;
        private MapSection dragStartMapSection;

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPxPos = ToPxPosInWindow(e.GetPosition(this));
            dragStartMapSection = _currentMapSection;
        }

        private (int x, int y) ToMapPxPos(Point mousePosition)
        {
            double mapPxPerDPI = _currentMapSection.DisplayWidth / MapImage.ActualWidth;
            var x = _currentMapSection.PosX + mousePosition.X * mapPxPerDPI;
            var y = _currentMapSection.PosY + mousePosition.Y * mapPxPerDPI;
            return ((int)x, (int)y);
        }

        private (int x, int y) ToPxPosInWindow(Point mousePosition)
        {
            double mapPxPerDPI = _currentMapSection.DisplayWidth / MapImage.ActualWidth;
            var x = mousePosition.X * mapPxPerDPI;
            var y = mousePosition.Y * mapPxPerDPI;
            return ((int)x, (int)y);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var mousePosGui = e.GetPosition((IInputElement)sender);
                var currentMousePos = ToPxPosInWindow(mousePosGui);

                int deltaX = dragStartPxPos.x - currentMousePos.x;
                int deltaY = dragStartPxPos.y - currentMousePos.y;

                Debug.WriteLine($"mouse at gui pos {mousePosGui}");
                Debug.WriteLine($"mouse at map pos {currentMousePos}");
                Debug.WriteLine($"map moved by {deltaX},{deltaY} from {dragStartMapSection}");

                SetPosition(dragStartMapSection.PosX + deltaX, dragStartMapSection.PosY + deltaY);
            //    UpdatePosition(deltaX, deltaY);
            //   dragStartPxPos = currentMousePos;
                RedrawMap(null, null);
            }
        }
    }
}
