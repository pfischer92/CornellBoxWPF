using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.Light
{
    public class LightSphere : LightSource
    {
        public float _Radius { get; set; }
        public LightSphere(Vector3 position, Vector3 color, float radius)
        {
            this._position = position;
            this._color = color;
            this._Radius = radius;
        }
    }
}
