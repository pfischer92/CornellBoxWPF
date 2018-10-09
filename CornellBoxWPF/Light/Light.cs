using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
