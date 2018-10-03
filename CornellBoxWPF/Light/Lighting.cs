using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class Lighting
    {
        List<Light> Lights = new List<Light>();


        public Lighting(List<Light> lights)
        {
            Lights = lights;
        }
    }
}
