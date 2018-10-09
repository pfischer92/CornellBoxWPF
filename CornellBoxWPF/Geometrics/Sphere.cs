using System.Numerics;

namespace CornellBoxWPF
{
    public class Sphere
    {
        public Vector3 _center { get; set; }
        public float _radius { get; set; }
        public Vector3 _color { get; set; }
        public bool _reflection { get; set; }
        public bool _proceduralTexture { get; set; }

        public bool _bitmapTexture { get; set; }
        public Sphere(Vector3 center, float radius, Vector3 color, bool reflection = false, bool procedualTexture = false, bool bitmapTexture = false)
        {
            _center = center;
            _radius = radius;
            _color = color;
            _reflection = reflection;
            _proceduralTexture = procedualTexture;
            _bitmapTexture = bitmapTexture;
        }
    }
}
