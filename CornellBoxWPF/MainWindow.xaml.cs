using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public class CheckBoxControl : INotifyPropertyChanged
        {
            private MainWindow _window;
            private bool isDiffuseCheckBoxChecked;
            private bool isSpecularCheckBoxChecked;
            private bool isShadowCheckBoxChecked;
            private bool isReflectionCheckBoxChecked;
            private bool isProceduralTextureCheckBoxChecked;
            private bool isBitmapTextureCheckBoxChecked;


            public bool IsDiffuseCheckBoxChecked
            {
                get
                {
                    return isDiffuseCheckBoxChecked;
                }

                set
                {
                    if (isDiffuseCheckBoxChecked != value)
                    {
                        isDiffuseCheckBoxChecked = value;
                        NotifyPropertChanged("isDiffuseCheckBoxChecked has changed");
                    }
                }
            }

            public bool IsSpecularCheckBoxChecked
            {
                get
                {
                    return isSpecularCheckBoxChecked;
                }

                set
                {
                    if (isSpecularCheckBoxChecked != value)
                    {
                        isSpecularCheckBoxChecked = value;
                        NotifyPropertChanged("isSpecularCheckBoxChecked has changed");
                    }
                }
            }

            public bool IsShadowCheckBoxChecked
            {
                get
                {
                    return isShadowCheckBoxChecked;
                }

                set
                {
                    if (isShadowCheckBoxChecked != value)
                    {
                        isShadowCheckBoxChecked = value;
                        NotifyPropertChanged("isSpecularCheckBoxChecked has changed");
                    }
                }
            }
            public bool IsReflectionCheckBoxChecked
            {
                get
                {
                    return isReflectionCheckBoxChecked;
                }

                set
                {
                    if (isReflectionCheckBoxChecked != value)
                    {
                        isReflectionCheckBoxChecked = value;
                        NotifyPropertChanged("isReflectionCheckBoxChecked has changed");
                    }
                }
            }

            public bool IsProceduralTextureCheckBoxChecked
            {
                get
                {
                    return isProceduralTextureCheckBoxChecked;
                }

                set
                {
                    if (isProceduralTextureCheckBoxChecked != value)
                    {
                        isProceduralTextureCheckBoxChecked = value;
                        NotifyPropertChanged("isProceduralTextureCheckBoxChecked has changed");
                    }
                }
            }

            public bool IsBitmapTextureCheckBoxChecked
            {
                get
                {
                    return isBitmapTextureCheckBoxChecked;
                }

                set
                {
                    if (isBitmapTextureCheckBoxChecked != value)
                    {
                        isBitmapTextureCheckBoxChecked = value;
                        NotifyPropertChanged("isBitmapTextureCheckBoxChecked has changed");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertChanged(string propertyName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                _window.PrintScene();
            }

            public CheckBoxControl(MainWindow window)
            {
                isDiffuseCheckBoxChecked = false;
                isSpecularCheckBoxChecked = false;
                isShadowCheckBoxChecked = false;
                isReflectionCheckBoxChecked = false;
                isProceduralTextureCheckBoxChecked = false;
                isBitmapTextureCheckBoxChecked = false;
                _window = window;
            }
        }
        public static WriteableBitmap image { get; set; }

        public static float GAMMA = 2.2f;
        public static int bytesPerPixel = 3;
        public static int antiAliasing = 2;

        public static CheckBoxControl checkBoxControl;

        public static List<Sphere> spheres = new List<Sphere>{ new Sphere(new Vector3(-1001, 0, 0), 1000, new Vector3(0, 0, 1)),
                                    new Sphere(new Vector3(1001, 0, 0), 1000, new Vector3(1, 0, 0)),
                                    new Sphere(new Vector3(0, 0, 1001), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, -1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, 1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3f, new Vector3(1, 1, 0), true),
                                    new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6f, new Vector3(1, 0, 1), true)};


        public static Lighting lights = new Lighting(new List<Light> { new Light(new Vector3(0.0f, -0.9f, 0), new Vector3(1.0f, 1.0f, 1.0f)),      // White light
                                                                       new Light(new Vector3(-0.4f, -0.9f, 0), new Vector3(0.0f, 0.0f, 1.0f)),    // Blue light
                                                                       new Light(new Vector3(0.4f, -0.9f, 0), new Vector3(1.0f, 1.0f, 1.0f))});  // Red light
        public static Scene scene = new Scene(spheres, lights);
        public static Gaussian rd = new Gaussian();

        public MainWindow()
        {
            image = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Rgb24, null);
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            checkBoxControl = new CheckBoxControl(this);
            DataContext = checkBoxControl;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PrintScene();
        }

        public void PrintScene()
        {
            byte[] colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    Vector3 color = Vector3.Zero;
                    for (int i = 0; i < antiAliasing; i++)
                    {
                        float tmp_x = (float)rd.RandomGauss(x, 0.5f);
                        float tmp_y = (float)rd.RandomGauss(y, 0.5f);
                        Vector2 coord = new Vector2((float)2.0 / image.PixelWidth * tmp_x - 1, (float)2.0 / image.PixelHeight * tmp_y - 1);
                        color += scene.CalcColour(checkBoxControl, scene.CreateEyeRay(coord));
                    }

                    color = color / antiAliasing;
                    
                    colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel)] = ConvertAndClampAndGammaCorrect(color.X);            // Red
                    colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 1)] = ConvertAndClampAndGammaCorrect(color.Y);        // Blue
                    colourData[(x * bytesPerPixel + y * image.PixelHeight * bytesPerPixel + 2)] = ConvertAndClampAndGammaCorrect(color.Z);        // Green
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            img.Source = image;
        }

        private static byte ConvertAndClampAndGammaCorrect(float color)
        {
            float gamma_corrected = (float)Math.Pow(color, 1f / GAMMA);
            int x = (int)Math.Round(255 * gamma_corrected, 0);

            if (x > 255) return 255;
            if (x < 0) return 0;
            return (byte)x;
        }
    }
}



