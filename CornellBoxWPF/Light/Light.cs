using System.Numerics;

namespace CornellBoxWPF
{
    public class Light
    {

        public Vector3 _position { get; set; }

        public Vector3 _color { get; set; }

        public Light(Vector3 position, Vector3 color)
        {
            _position = position;
            _color = color;
        }
    }
}
