using System.Drawing;
using System.Numerics;

namespace CornellBoxWPF.BitmapHelper
{
    public static class BitmapTexturing
    {
        private static Bitmap bitmap = new Bitmap("wall.bmp");

        public static Vector3 GetColor(float x, float y)
        {
            Color c = bitmap.GetPixel((int)x * bitmap.Width, (int)y * bitmap.Height);
            return new Vector3(1 / 255 * c.R, 1 / 255 * c.G, 1 / 255 * c.B);
        }
    }
}
