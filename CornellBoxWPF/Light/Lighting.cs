using System.Collections.Generic;

namespace CornellBoxWPF
{
    public class Lighting
    {
        public List<LightSource> _lights { get; set; }

        public Lighting(List<LightSource> lights)
        {
            _lights = lights;
        }

        public Lighting() { }
    }
}
