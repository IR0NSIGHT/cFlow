using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.components;

public partial class MapView
{
    private class MapPositioner
    {
        public EventHandler<Int32Rect> OnMapSectionChanged;

        public MapPositioner(Grid mapWindow)
        {
            mapWindow.MouseMove += OnPreviewMouseMove;
            mapWindow.MouseRightButtonDown += OnPreviewMouseRightButtonDown;
            mapWindow.MouseWheel += PreviewMouseWheel;
            this.mapView = mapWindow;
        }

        public void SetMapDimensions((int width, int height) mapDimensions)
        {
            this.mapDimensions = mapDimensions;
            SetSectionByScale(currentScale);
        }

        public float CurrentScale => currentScale;

        private (int width, int height) mapDimensions;
        public (int width, int height) MapDimensions => mapDimensions;
        private Grid mapView;
        private (int x, int y) dragStartPxPos;
        private MapSection dragStartMapSection;
        private float currentScale = 1;
        private MapSection _currentMapSection = new();

        public record struct MapSection(int PosX, int PosY, int DisplayWidth)
        {
            public int PosX = PosX;
            public int PosY = PosY;
            public int DisplayWidth = DisplayWidth;
        }


        public Int32Rect getDisplayedAreaOfMapImage()
        {
            (double guiWidth, double guiHeight) = (mapView.ActualWidth, mapView.ActualHeight);
            int xWidth = _currentMapSection.DisplayWidth;
            int yHeight = (int)(_currentMapSection.DisplayWidth * (guiHeight / (guiWidth)));
            int xPos = (int)Math.Clamp(_currentMapSection.PosX, 0, mapDimensions.width - xWidth);
            int yPos = (int)Math.Clamp(_currentMapSection.PosY, 0, mapDimensions.height - yHeight);
            Debug.Assert(xWidth != 0 && yHeight != 0);
            Debug.Assert(xWidth + xPos <= mapDimensions.width);
            Debug.Assert(yHeight + yPos <= mapDimensions.height);

            return new Int32Rect(
                xPos,
                yPos,
                xWidth,
                yHeight
            );
        }
        private void SetSectionByScale(float scale)
        {
            _currentMapSection = _currentMapSection with { DisplayWidth = (int)(mapDimensions.width / scale) };
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
            this.OnMapSectionChanged.Invoke(this, getDisplayedAreaOfMapImage());
        }

        private MapSection SectionWithPosition(int x, int y)
        {
            return _currentMapSection with
            {
                PosX = Math.Clamp(x, 0, mapDimensions.width),
                PosY = Math.Clamp(y, 0, mapDimensions.height)
            };
        }
        
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IInputElement input))
                return;

            dragStartPxPos = ToPxPosInWindow(e.GetPosition(input));
            dragStartMapSection = _currentMapSection;
        }

        public (int x, int y) ToMapPxPos(System.Windows.Point positionInWindow)
        {
            var (x, y) = ToPxPosInWindow(positionInWindow);
            return (_currentMapSection.PosX + x, _currentMapSection.PosY + y);
        }

        public (int x, int y) ToPxPosInWindow(System.Windows.Point positionInWindow)
        {
            double mapPxPerDPI = _currentMapSection.DisplayWidth / mapView.ActualWidth;
            var x = positionInWindow.X * mapPxPerDPI;
            var y = positionInWindow.Y * mapPxPerDPI;
            return ((int)x, (int)y);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var mousePosGui = e.GetPosition((IInputElement)sender);
                var currentMousePos = ToPxPosInWindow(mousePosGui);

                //moved mouse this many map units since start of dragging
                int deltaX = dragStartPxPos.x - currentMousePos.x;
                int deltaY = dragStartPxPos.y - currentMousePos.y;

                _currentMapSection = SectionWithPosition(dragStartMapSection.PosX + deltaX, dragStartMapSection.PosY + deltaY);
                OnMapSectionChanged.Invoke(this, getDisplayedAreaOfMapImage());
            }
        }
    }
}