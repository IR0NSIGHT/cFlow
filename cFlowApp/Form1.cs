﻿using System;
using System.Drawing;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace cFlowApp
{
    public partial class Form1 : Form
    {
        private SKControl skControl;
        string folder = "C:\\Users\\Max1M\\\\OneDrive\\Bilder\\cFlow\\";
        string file = "medium_flats.png";
        public Form1()
        {
            InitializeComponent();
            var image = new Bitmap(folder + file);
            var testString = ClassLibrary1.MyTestClassApi.myString;
            Console.WriteLine(testString);
            //var cFlow = new cFlowApi.CFlowGenerator(folder + file);

            ShowMyImage(pictureBox1, image );

        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // You can leave this event empty or handle additional painting logic if needed
        }

        public static void ShowMyImage(PictureBox box, Bitmap skBitmap)
        {
            var (xSize, ySize) = (box.Width, box.Height);

            // Calculate the new size while maintaining the aspect ratio
            float aspectRatio = (float)skBitmap.Width / skBitmap.Height;
            int newHeight;
            int newWidth;
            if (skBitmap.Width > skBitmap.Height)
            {
                //more wide than high
                newWidth = xSize;
                newHeight = (int)(xSize / aspectRatio);

            }
            else
            {
                //more high than wide
                newHeight = ySize;
                newWidth = (int)(ySize / aspectRatio);
            }


            //center image that might be smaller than box
            box.Location = CenterPosResizedImage(box.Location, (xSize, ySize), (newWidth, newHeight));

            // Stretches the image to fit the pictureBox.
            box.SizeMode = PictureBoxSizeMode.StretchImage;
            box.ClientSize = new Size(newWidth, newHeight);

            box.Image = (Image)skBitmap;

        }

        /// <summary>
        /// image has oldPos with oldSize. redraw image to another size, have to recalc position so the new image is centered to the old images center
        /// built for resizing to smaller images.
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="oldSize"></param>
        /// <param name="newSize"></param>
        /// <returns>new position of image when using new size</returns>
        private static Point CenterPosResizedImage(Point oldPos, (int x, int y) oldSize, (int x, int y) newSize)
        {
            //center image that might be smaller than box
            int xPosition = (oldSize.x - newSize.x) / 2;
            int yPosition = (oldSize.y - newSize.y) / 2;
            return new Point(oldPos.X + xPosition, oldPos.Y + yPosition);
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