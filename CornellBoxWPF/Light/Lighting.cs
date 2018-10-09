using System.Collections.Generic;

namespace CornellBoxWPF
{
    public class Lighting
    {
        public List<Light> _lights { get; set; }

        public Lighting(List<Light> lights)
        {
            _lights = lights;
        }

        public Lighting() { }
    }
}
