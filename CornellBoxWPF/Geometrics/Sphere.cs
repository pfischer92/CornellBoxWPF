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
        public Vector3 _center { get; set; }
        public float _radius { get; set; }
        public Vector3 _color { get; set; }
        public bool _reflection { get; set; }
        public bool _proceduralTexture { get; set; }

        public bool BitmapTexture { get; set; }
        public Sphere(Vector3 center, float radius, Vector3 color, bool reflection = false, bool procedualTexture = false, bool bitmapTexture = false)
        {
            _center = center;
            _radius = radius;
            _color = color;
            _reflection = reflection;
            _proceduralTexture = procedualTexture;
            BitmapTexture = bitmapTexture;
        }
    }
}
