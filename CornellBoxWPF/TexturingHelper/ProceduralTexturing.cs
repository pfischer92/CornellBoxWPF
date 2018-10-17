using CornellBoxWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF.TexturingHelper
{
    public class ProceduralTexturing
    {
        public static Vector3 GetProceduralColor(double x, double y, double z)
        {
            if (((x - Math.Truncate(x)) + (y - Math.Truncate(y)) + (z - Math.Truncate(z))) < 0.5)
            {
                return new Vector3(GammaCorrection.ConvertAndClampAndGammaCorrect(1.0f), 0, 0);
            }
            else
            {
                return new Vector3(0, 0, 0);
            }  
        }
    }
}
