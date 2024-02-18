﻿using System.Drawing;
using System.Drawing.Imaging;
using application.Maps.heightMap;

namespace unittest;
[TestFixture]
public class ShadedHeightmapComputerTest
{
    [Test]
    public void owo()
    {
        Bitmap bitmap = new Bitmap("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\medium_flats_gray8bpp.png");

        Bitmap shaded = ShadedHeightmapComputer.RunShader(bitmap);
        shaded.Save("C:\\Users\\Max1M\\OneDrive\\Bilder\\cFlow\\shaded.png", ImageFormat.Png);
    }
}