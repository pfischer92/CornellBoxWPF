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
                _window = window;
            }
        }
        public WriteableBitmap image { get; set; }

        public static float GAMMA = 2.2f;

        public CheckBoxControl checkBoxControl;

        public MainWindow()
        {
            image = new WriteableBitmap(600, 600, 96, 96, PixelFormats.Bgr32, null);
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            checkBoxControl = new CheckBoxControl(this);
            DataContext = checkBoxControl;
        }

        public List<Sphere> spheres = new List<Sphere>{ new Sphere(new Vector3(-1001, 0, 0), 1000, new Vector3(0, 0, 1)),
                                    new Sphere(new Vector3(1001, 0, 0), 1000, new Vector3(1, 0, 0)),
                                    new Sphere(new Vector3(0, 0, 1001), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, -1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(0, 1001, 0), 1000, new Vector3(1, 1, 1)),
                                    new Sphere(new Vector3(-0.6f, 0.7f, -0.6f), 0.3f, new Vector3(1, 1, 0), true),
                                    new Sphere(new Vector3(0.3f, 0.4f, 0.3f), 0.6f, new Vector3(1, 0, 1), true)};

                        

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PrintScene();
        }

        public void PrintScene()
        {
            Scene scene = new Scene(spheres);
            int bytesPerPixel = 4;
            byte[] colourData = new byte[image.PixelHeight * image.PixelWidth * bytesPerPixel];

            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    Vector3 color = scene.CalcColour(checkBoxControl, scene.CreateEyeRay(new Vector2((float)2.0 / image.PixelWidth * x - 1, (float)2.0 / image.PixelHeight * y - 1)));
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel)] = ConvertAndClampAndGammaCorrect(color.Z);            // Blue
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 1)] = ConvertAndClampAndGammaCorrect(color.Y);        // Green
                    colourData[(x * 4 + y * image.PixelHeight * bytesPerPixel + 2)] = ConvertAndClampAndGammaCorrect(color.X);        // Red
                }
            }

            image.Lock();
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), colourData, image.PixelWidth * bytesPerPixel, 0);
            image.Unlock();

            img.Source = image;
        }

        private byte ConvertAndClampAndGammaCorrect(float color)
        {
            float gamma_corrected = (float)Math.Pow(color, 1f / GAMMA);
            int x = (int)Math.Round(255 * gamma_corrected, 0);

            if (x > 255)
                return 255;

            if (x < 0)
                return 0;

            return (byte)x;
        }
    }
}



