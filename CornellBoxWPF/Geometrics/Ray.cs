using System.Numerics;

namespace CornellBoxWPF
{
    public class Ray
    {
        public Vector3 _origin { get; set; }
        public Vector3 _direction { get; set; }
        public Ray(Vector3 origin, Vector3 direction)
        {
            _origin = origin;
            _direction = direction;
        }
    }
}
