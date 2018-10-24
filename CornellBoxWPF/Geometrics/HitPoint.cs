using System.Numerics;

namespace CornellBoxWPF
{
    public class HitPoint
    {
        public Ray _ray { get; set; }
        public Vector3 _point { get; set; }
        public Vector3 _color { get; set; }
        public Sphere _sphere { get; set; }

        public HitPoint(Ray ray, Vector3 h, Vector3 color, Sphere sphere)
        {
            _ray = ray;
            _point = h;
            _color = color;
            _sphere = sphere;
        }
    }
}
