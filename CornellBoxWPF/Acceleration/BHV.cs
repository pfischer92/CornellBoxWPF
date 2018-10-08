using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.Acceleration
{
    public class BHV
    {
        private List<Sphere> Spheres { get; set; } 
        public BHV(List<Sphere> spheres)
        {
            Spheres = spheres;
        }


    }
}
