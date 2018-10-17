using CornellBoxWPF.Helpers;
using System;
using System.Drawing;
using System.Numerics;

namespace CornellBoxWPF.BitmapHelper
{
    public class BitmapTexturing
    {
        Bitmap bitmap;
        public BitmapTexturing()
        {
            bitmap = (Bitmap)Bitmap.FromFile("sample-bitmap.bmp");
        }
        public Vector3 GetBitmapColor(double x, double y, double z)
        {
            double s = Math.Atan2(x, z);
            double t = Math.Acos(y);

            // Get Color from bitmap
            int u = (int)(t / (Math.PI) * bitmap.Width);
            int v = (int)((s + Math.PI) / (2 * Math.PI) * bitmap.Height);

            if (u > 0 && u < bitmap.Width && v > 0 && v < bitmap.Height)
            {
                Color c = bitmap.GetPixel(u, v);
                Vector3 bitMapColor = Vector3.Normalize(new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(c.R), GammaCorrection.ConvertAndClampAndGammaCorrect(c.G), GammaCorrection.ConvertAndClampAndGammaCorrect(c.B)));
                return bitMapColor;
            }
            else
            {
                return new Vector3(1, 1, 1);    // Black
            }
        }
    }
}
