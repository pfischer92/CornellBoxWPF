using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class Sphere
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public Vector3 Color { get; set; }
        public bool Reflection { get; set; }
        public Sphere(Vector3 center, float radius, Vector3 color, bool reflection = false)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Reflection = reflection;
        }
        public Sphere()
        {
        }
    }
}
