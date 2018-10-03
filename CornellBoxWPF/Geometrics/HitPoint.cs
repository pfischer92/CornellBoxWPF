using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class HitPoint
    {
        public Ray Ray { get; set; }
        public Vector3 H { get; set; }
        public Vector3 Color { get; set; }
        public Sphere Sphere { get; set; }

        public HitPoint(Ray ray, Vector3 h, Vector3 color, Sphere sphere)
        {
            Ray = ray;
            H = h;
            Color = color;
            Sphere = sphere;
        }
    }
}
