using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class Lighting
    {
        public List<Light> Lights { get; set; }

        public Lighting(List<Light> lights)
        {
            Lights = lights;
        }

        public Lighting() { }
    }
}
