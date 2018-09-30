using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CornellBoxWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WriteableBitmap image { get; set; }

        public MainWindow()
        {
            image = new WriteableBitmap(1000, 1000, 96, 96, PixelFormats.Bgr32, null);
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public static Vector3 eye = new Vector3(0, 0, -4);
        public static Vector3 lookAt = new Vector3(0, 0, 6);
        public static Vector3 up = new Vector3(0, 1, 0);
        public static Vector3 lightPos = new Vector3(0.0f, -0.9f, 0);
        public static Vector3 lightIntensity = new Vector3(1.0f, 1.0f, 1.0f);
        public static float FOV = (float)(36 * Math.PI / 180);
        public static int k = 40;

        public Sphere[] scene = { new Sphere(new Vector3(-1001, 0, 0), 1000, new Vector3(0, 0, 1)),
                                    new Sphere(new Vector3(1001, 0, 0), 1000, new Vector3(1, 0, 0)),
                                    new Sphere(new Vector3(0, 0, 1001), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, -1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, 1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3f, new Vector3(1, 1, 0)),
                                    new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6f, new Vector3(1, 0, 1))};

        public Ray CreateEyeRay(Vector3 eye, Vector3 lookAt, float FOV, Vector2 pixel)
        {
            Vector3 f = Vector3.Normalize(lookAt - eye);                      
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, up));
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));
            Vector3 d = f + pixel.X * r * (float)Math.Tan(FOV / 2) + pixel.Y * u * (float)Math.Tan(FOV / 2);

            return new Ray(eye, Vector3.Normalize(d));
        }

        public HitPoint FindClosestHitPoint(Sphere[] scene, Ray ray)
        {
            Vector3 H = new Vector3(0, 0, 0);
            Vector3 colour = new Vector3(0, 0, 0);
            int idxSmallestLambda = int.MaxValue;
            float smallestLambda = float.PositiveInfinity;

            for (int i = 0; i < scene.Length; i++)
            {
                float lambda = scene[i].FindHitPoint(ray);

                if (lambda > 0 && lambda < smallestLambda)
                {
                    H = ray.Origin + lambda * ray.Direction;
                    colour = scene[i].Color;
                    smallestLambda = lambda;
                    idxSmallestLambda = i;
                }
            }
            return new HitPoint(ray, H, colour, scene[idxSmallestLambda]);
        }

        public Vector3 CalcColour(Sphere[] scene, Ray ray)
        {
            HitPoint hitpoint = FindClosestHitPoint(scene, ray);
            return hitpoint.Color;
        }

        public Vector3 CalcColourWithDiffuse(Sphere[] scene, Ray ray)
        {
            HitPoint hitpoint = FindClosestHitPoint(scene, ray);
            Vector3 diffLight = Vector3.Zero;

            Vector3 n = Vector3.Normalize(Vector3.Subtract(hitpoint.H, hitpoint.Sphere.Center));
            Vector3 l = Vector3.Normalize(Vector3.Subtract(lightPos, hitpoint.H));
            float nL = Vector3.Dot(n, l);

            if(nL >= 0)
            {
                Vector3 ilm = Vector3.Multiply(lightIntensity, hitpoint.Sphere.Color);       // Normalize???        
                diffLight = Vector3.Multiply(ilm, nL);
            }
            return diffLight;
        }

        public Vector3 CalcColorWithDiffuseAndPhong(Sphere[] scene, Ray ray)
        {
            HitPoint hitpoint = FindClosestHitPoint(scene, ray);
            Vector3 phong = Vector3.Zero;
            
            Vector3 n = Vector3.Normalize(Vector3.Subtract(hitpoint.H, hitpoint.Sphere.Center));
            Vector3 l = Vector3.Normalize(Vector3.Subtract(lightPos, hitpoint.H));
            float nL = Vector3.Dot(n, l);

            if (nL >= 0)
            {                
                Vector3 EH = Vector3.Normalize(Vector3.Subtract(eye, hitpoint.H));
                Vector3 s = l - Vector3.Dot(l, n) * n; 
                Vector3 r = Vector3.Normalize(l - 2 * s);
                
                float phongFactor = (float)Math.Pow(Math.Max(0, Vector3.Dot(r, EH)), k);
                phong = lightIntensity * phongFactor;
            }

            return phong + CalcColourWithDiffuse(scene, ray);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int bytesPerPixel = 4; 
            byte[] colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];

            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    //Vector3 color = CalcColour(scene, CreateEyeRay(eye, lookAt, FOV, new Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));                      // Plain
                    //Vector3 color = CalcColourWithDiffuse(scene, CreateEyeRay(eye, lookAt, FOV, rnew Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));             // Diffuse Light
                    Vector3 color = CalcColorWithDiffuseAndPhong(scene, CreateEyeRay(eye, lookAt, FOV, new Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));        // Diffuse & Phong
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel)] = ConvertAndClamp8(color.Z);            // Blue
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 1)] = ConvertAndClamp8(color.Y);        // Green
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 2)] = ConvertAndClamp8(color.X);        // Red
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            img.Source = image;
        }

        private byte ConvertAndClamp8(float n)
        {
            int x = (int)Math.Round(255 * n, 0);

            if (x > 255)
                return 255;

            if (x < 0)
                return 0;

            return (byte)x;
        }
    }
}



