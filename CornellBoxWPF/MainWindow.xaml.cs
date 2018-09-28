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
        public static Vector3 light = new Vector3(0.0f, -0.9f, 0);
        public static Vector3 lightIntensity = new Vector3(1.0f, 1.0f, 1.0f);
        public static float FOV = (float)(36 * Math.PI / 180);
        public static int k = 1000;


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
            Vector3 l = Vector3.Normalize(Vector3.Subtract(light, hitpoint.H));
            float nL = Vector3.Dot(n, l);

            if(nL >= 0)
            {
                Vector3 ilm = Vector3.Multiply(lightIntensity, hitpoint.Sphere.Color);            
                diffLight = Vector3.Multiply(ilm, nL);
            }
            return diffLight;
        }

        public Vector3 CalcColorWithDiffuseAndPhong(Sphere[] scene, Ray ray)
        {
            HitPoint hitpoint = FindClosestHitPoint(scene, ray);
            Vector3 diffLight = Vector3.Zero;
            Vector3 phong = Vector3.Zero;

            Vector3 n = Vector3.Normalize(Vector3.Subtract(hitpoint.H, hitpoint.Sphere.Center));
            Vector3 l = Vector3.Normalize(Vector3.Subtract(light, hitpoint.H));
            float nL = Vector3.Dot(n, l);

            if (nL >= 0)
            {
                Vector3 ilm = Vector3.Multiply(lightIntensity, hitpoint.Sphere.Color);
                diffLight = Vector3.Multiply(ilm, nL);

                Vector3 EH = Vector3.Normalize(Vector3.Subtract(hitpoint.H, eye));
                Vector3 r = Vector3.Subtract(l, 2 * (Vector3.Dot(l, n)) * n);
                float fac = (float)Math.Pow((r.Length()*EH.Length()), k);         // Dot??
                phong = Vector3.Multiply(diffLight, fac);
            }
            return phong;            
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
                    //Vector3 color = CalcColourWithDiffuse(scene, CreateEyeRay(eye, lookAt, FOV, new Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));             // Diffuse Light
                    Vector3 color = CalcColorWithDiffuseAndPhong(scene, CreateEyeRay(eye, lookAt, FOV, new Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));        // Diffuse & Phong
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel)] = Convert.ToByte(color.X * 255);
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 1)] = Convert.ToByte(color.Y * 255);
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 2)] = Convert.ToByte(color.Z * 255);
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            img.Source = image;
        }

        public double GetAngle(Vector3 a, Vector3 b)
        {
            double theta = Math.Acos((Vector3.Dot(a,b)/(a.Length()*b.Length())));
            return theta;            
        }
    }
}



