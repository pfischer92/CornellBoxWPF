using System.Numerics;

namespace CornellBoxWPF
{
    public class LightSource
    {
        public Vector3 _position { get; set; }

        public Vector3 _color { get; set; }

        public LightSource(Vector3 position, Vector3 color)
        {
            _position = position;
            _color = color;
        }
        public LightSource()
        {
        }
    }
}
