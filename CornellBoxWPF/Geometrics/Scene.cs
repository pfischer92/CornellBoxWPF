using System;
using System.Collections.Generic;
using System.Numerics;
using static CornellBoxWPF.MainWindow;

namespace CornellBoxWPF
{
    public class Scene
    {
        public static Vector3 eye = new Vector3(0, 0, -4);
        public static Vector3 lookAt = new Vector3(0, 0, 6);
        public static Vector3 up = new Vector3(0, 1, 0);
        //public static Vector3 lightPos = new Vector3(0.0f, -0.9f, 0);
        //public static Vector3 lightIntensity = new Vector3(1.0f, 1.0f, 1.0f);
        public static float FOV = (float)(36 * Math.PI / 180);
        public static int k = 40;
        public static int reflectionFactorMax = 2;
        public static int reflectionStep = 0;

        private List<Sphere> Spheres = new List<Sphere>();
        private Lighting Lighting = new Lighting();

        public Scene(List<Sphere> spheres, Lighting lighting)
        {
            Spheres = spheres;
            Lighting = lighting;
        }

        public Ray CreateEyeRay(Vector2 pixel)
        {
            Vector3 f = Vector3.Normalize(lookAt - eye);
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, up));
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));
            Vector3 d = f + pixel.X * r * (float)Math.Tan(FOV / 2) + pixel.Y * u * (float)Math.Tan(FOV / 2);

            return new Ray(eye, Vector3.Normalize(d));
        }

        public float FindHitPoint(Ray ray, Sphere sp)
        {
            var a = 1;
            var cE = ray.Origin - sp.Center;
            var b = 2 * Vector3.Dot(cE, ray.Direction);
            var c = cE.Length() * cE.Length() - sp.Radius * sp.Radius;
            var discriminant = b * b - 4 * a * c;
            var determinant = Math.Sqrt(discriminant);

            if (discriminant >= 0)
            { //at least one hit

                var lambda1 = (-b + determinant) / (2 * a);
                var lambda2 = (-b - determinant) / (2 * a);

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

        public HitPoint FindClosestHitPoint(Ray ray)
        {
            Vector3 H = new Vector3(0, 0, 0);
            Vector3 colour = new Vector3(0, 0, 0);
            Sphere closestSphere = new Sphere();
            float smallestLambda = float.PositiveInfinity;

            foreach(Sphere sp in Spheres)
            {
                float lambda = FindHitPoint(ray, sp);

                if (lambda > 0 && lambda < smallestLambda)
                {
                    H = ray.Origin + lambda * ray.Direction;
                    colour = sp.Color;
                    smallestLambda = lambda;
                    closestSphere = sp;
                }
            }
            return new HitPoint(ray, H, colour, closestSphere);
        }

        public Vector3 CalcColour(CheckBoxControl checkBoxControl, Ray ray, int reflectionFactor = 0)
        {            
            HitPoint hitpoint = FindClosestHitPoint(ray);
            Vector3 color = new Vector3(0, 0, 0);

            // No hitpoint found
            if (hitpoint == null){ return Vector3.Zero;}

            foreach (Light light in Lighting.Lights)
            {
                // Get overall settings
                Vector3 n = Vector3.Normalize(hitpoint.H - hitpoint.Sphere.Center);     // Normal vector of hitpoint
                Vector3 l = Vector3.Normalize(Vector3.Subtract(light.Position, hitpoint.H));
                float nL = Vector3.Dot(n, l);
                Vector3 s = l - Vector3.Dot(l, n) * n;
                Vector3 EH = Vector3.Normalize(Vector3.Subtract(eye, hitpoint.H));
                Vector3 r = Vector3.Normalize(l - 2 * s);

                // Case 1: Simple Ray Tracing 
                color = hitpoint.Color;

                // Case 2: Diffuse7Lambert Light
                if (checkBoxControl.IsDiffuseCheckBoxChecked)
                {
                    Vector3 diffLight = Vector3.Zero;

                    if (nL >= 0)
                    {
                        diffLight = light.Color * hitpoint.Sphere.Color * nL;
                    }
                    color = diffLight;
                }

                // Case 3: Phong/Specular
                if (checkBoxControl.IsSpecularCheckBoxChecked)
                {
                    if (nL >= 0)
                    {
                        float phongFactor = (float)Math.Pow(Math.Max(0, Vector3.Dot(r, EH)), k);
                        Vector3 phong = light.Color * phongFactor;
                        color = color + phong;
                    }
                }

                // Case 4: Shadows
                if (checkBoxControl.IsShadowCheckBoxChecked)
                {
                    Ray lightRay = new Ray(hitpoint.H, Vector3.Normalize(light.Position - hitpoint.H));
                    lightRay.Origin += lightRay.Direction * 0.001f;
                    HitPoint shadow = FindClosestHitPoint(lightRay);

                    if (shadow != null && (shadow.H - hitpoint.H).Length() < (light.Position - hitpoint.H).Length())
                    {
                        color -= light.Color * color * 0.5f;
                    }
                }
            }
            // Case 5: Reflections
            if (checkBoxControl.IsReflectionCheckBoxChecked)
                {
                    if (hitpoint.Sphere.Reflection && reflectionStep < 10)
                    {
                        Vector3 l1 = Vector3.Normalize(eye - hitpoint.H);
                        Vector3 n2 = Vector3.Normalize(hitpoint.H - hitpoint.Sphere.Center);
                        Vector3 r2 = 2 * (Vector3.Dot(l1, n2)) * n2 - l1;

                        Vector3 col = CalcColour(checkBoxControl, new Ray(eye, r2), reflectionStep + 1);

                        if (col.X > 0 || col.Y > 0 || col.Z > 0)
                        {
                            color += col * 0.4f;
                        }
                    }
                }
            
            return color;
        }
    }
}
