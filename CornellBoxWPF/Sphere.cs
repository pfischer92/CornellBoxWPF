using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CornellBoxWPF
{
    public class Sphere
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public Vector3 Color { get; set; }
        public Sphere(Vector3 center, float radius, Vector3 color)
        {
            Center = center;
            Radius = radius;
            Color = color;
        }

        public float FindHitPoint(Ray ray)
        {
            var a = 1;
            var cE = ray.Origin - this.Center;
            var b = 2 * Vector3.Dot(cE, ray.Direction);
            var c = cE.Length() * cE.Length() - this.Radius * this.Radius;
            var discriminant = b * b - 4 * a * c;

            if (discriminant >= 0)
            { //at least one hit

                var lambda1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                var lambda2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

                if (lambda1 > 0 && lambda2 > 0)
                {
                    return (float)Math.Min(lambda1, lambda2);
                }
                else if (lambda1 > 0)
                {
                    return (float)lambda1;
                }
                else if (lambda2 > 0)
                {
                    return (float)lambda2;
                }
            }
            return 0;
        }
    }
}
