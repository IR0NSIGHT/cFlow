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
        private ScrollViewer mapScrollViewer;
        public MapPositioner(Canvas mapWindow, ScrollViewer mapScrollViewer)
        {
            mapWindow.MouseMove += OnPreviewMouseMove;
            mapWindow.MouseRightButtonDown += OnPreviewMouseRightButtonDown;
            this.MapCanvas = mapWindow;
            this.mapScrollViewer = mapScrollViewer;

            mapScrollViewer.PanningMode = PanningMode.None;
        }

        public void SetMapDimensions((int width, int height) mapDimensions)
        {
            this.mapDimensions = mapDimensions;
            SetSectionByScale(currentScale);
        }

        public float CurrentScale => currentScale;

        private (int width, int height) mapDimensions;
        public (int width, int height) MapDimensions => mapDimensions;
        private Canvas MapCanvas;
        private (int x, int y) dragStartPxPos;
        private float currentScale = 1;
        private MapSection _currentMapSection = new();

        public record struct MapSection(int PosX, int PosY, int DisplayWidth)
        {
            public int PosX = PosX;
            public int PosY = PosY;
            public int DisplayWidth = DisplayWidth;
        }

        public Int32Rect MapAsRect()
        {
            return new Int32Rect(
                0,
                0,
                mapDimensions.width,
                mapDimensions.height
            );
        }

        private void SetSectionByScale(float scale)
        {
            _currentMapSection = _currentMapSection with { DisplayWidth = (int)(mapDimensions.width / scale) };
        }

        //returns newScale/oldScale
        private float updateScale(float scaleMultiplier)
        {
            var oldScale = currentScale;
            this.currentScale = Math.Clamp(currentScale * scaleMultiplier, 0.01f, 1000);
            //Debug.WriteLine($"Set map scale to {currentScale}");
            return currentScale / oldScale;
        }

        public void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mouseCanvasPos = e.GetPosition(MapCanvas);
            var multi = e.Delta < 0 ? 1/1.1f : 1.1f;
            var scaleDelta = updateScale(multi);
            var newMousePos = (mouseCanvasPos.X * scaleDelta, mouseCanvasPos.Y * scaleDelta);
            var movedDelta = (mouseCanvasPos.X - newMousePos.Item1, mouseCanvasPos.Y - newMousePos.Item2);

            var oldCanvasOffset = 
                (mapScrollViewer.HorizontalOffset, mapScrollViewer.VerticalOffset);

            var newCanvasOffset = 
                (oldCanvasOffset.HorizontalOffset - movedDelta.Item1,
                oldCanvasOffset.VerticalOffset - movedDelta.Item2);

            //move map to
            mapScrollViewer.ScrollToHorizontalOffset(newCanvasOffset.Item1);
            mapScrollViewer.ScrollToVerticalOffset(newCanvasOffset.Item2);

            this.OnMapSectionChanged.Invoke(this, MapAsRect());
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is IInputElement input))
                return;

            dragStartPxPos = ToPxPosInWindow(e);
        //    Debug.WriteLine($"SET ANCHOR TO {dragStartPxPos}");
        }

        public (int x, int y) ToMapPxPos(MouseEventArgs args)
        {
            var (x, y) = ToPxPosInWindow(args);
            return (_currentMapSection.PosX + x, _currentMapSection.PosY + y);
        }

        public (int x, int y) ToPxPosInWindow(MouseEventArgs args)
        {
            var positionInWindow = args.GetPosition(MapCanvas);
            double mapPxPerDPI = _currentMapSection.DisplayWidth / MapCanvas.ActualWidth;
            var x = positionInWindow.X * mapPxPerDPI;
            var y = positionInWindow.Y * mapPxPerDPI;
            return ((int)x, (int)y);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            //FIXME disabled for now: scrollview handles movement
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var mouseMapCoord = ToPxPosInWindow(e);

                //moved mouse this many map units since start of dragging
                int deltaX = dragStartPxPos.x - mouseMapCoord.x;
                int deltaY = dragStartPxPos.y - mouseMapCoord.y;

            //    Debug.WriteLine($"map move from anchor {dragStartPxPos} to now {mouseMapCoord}");
                mapScrollViewer.ScrollToHorizontalOffset(mapScrollViewer.HorizontalOffset + deltaX);
                mapScrollViewer.ScrollToVerticalOffset(mapScrollViewer.VerticalOffset + deltaY);
                OnMapSectionChanged.Invoke(this, MapAsRect());
            }
        }
    }
}