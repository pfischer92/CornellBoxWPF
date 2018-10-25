using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CornellBoxWPF.Acceleration
{
    public class BHVAccelaration
    {
        public List<Sphere> spheres { get; }
        
        public BHVAccelaration(List<Sphere> spheres)
        {
            this.spheres = new List<Sphere>(spheres.ToArray()); // Deep copy
        }

        public List<Sphere> CreateTreeStructure()
        {
            Sphere sphere1 = null;
            Sphere sphere2 = null;
            while(spheres.Count > 1)
            {
                float minDistance = float.MaxValue;
                float distance;
                foreach (Sphere s1 in spheres)
                {
                    foreach (Sphere s2 in spheres)
                    {
                        // Pick min BV- Pair
                        if (!s1.Equals(s2)) // Ignore same sphere
                        {
                            distance = (float)Math.Sqrt((float)(Math.Pow(s1._center.X - s2._center.X, 2) + Math.Pow(s1._center.Y - s2._center.Y, 2) + Math.Pow(s1._center.Z - s2._center.Z, 2)));
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                sphere1 = s1;
                                sphere2 = s2;
                            }
                        }
                    }
                }

                // Create new BHV Pair
                float radius = ((sphere1._color + sphere2._center).Length() + sphere1._radius + sphere2._radius) / 2f;
                Vector3 center = sphere1._center + Vector3.Normalize(sphere1._center - sphere2._center) * (radius - sphere1._radius);
                BHVSphere bHVSphere = new BHVSphere(center, radius, Vector3.Zero, sphere1, sphere2);

                // Update spheres
                spheres.Remove(sphere1);
                spheres.Remove(sphere2);
                spheres.Add(bHVSphere);
            }


            return spheres;
        }
    }
}
