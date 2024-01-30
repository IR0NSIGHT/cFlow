using System.Drawing;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace cFlowApp
{
    public partial class Form1 : Form
    {
        private SKControl skControl;

        public Form1()
        {
            InitializeComponent();

            // Manually create an SKControl
            skControl = new SKControl();
            skControl.PaintSurface += skControl1_PaintSurface;
            skControl.Size = new Size(600, 600); // Set your desired size

            // Add the SKControl to the form
            Controls.Add(skControl);
            // Force a redraw
            skControl.Invalidate();

        }

        private void skControl1_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            // Your SkiaSharp drawing code goes here
            using (SKPaint paint = new SKPaint())
            {
                paint.Color = SKColors.Blue;
                canvas.DrawRect(new SKRect(10, 10, 500, 500), paint);
            }
        }
    }
}