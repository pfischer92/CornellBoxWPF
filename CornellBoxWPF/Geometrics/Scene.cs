using CornellBoxWPF.BitmapHelper;
using CornellBoxWPF.TexturingHelper;
using System;
using System.Collections.Generic;
using System.Numerics;
using static CornellBoxWPF.MainWindow;

namespace CornellBoxWPF
{
    public class Scene
    {
        public static Vector3 _eye = new Vector3(0, 0, -4);
        public static Vector3 _lookAt = new Vector3(0, 0, 6);
        public static Vector3 _up = new Vector3(0, 1, 0);
        public static float _FOV = (float)(36 * Math.PI / 180);
        public static int _k = 40;
        public static int _reflectionStep = 5;

        private List<Sphere> _spheres = new List<Sphere>();
        private Lighting _lighting = new Lighting();
        BitmapTexturing bitmapTexturing = new BitmapTexturing();

        public Scene(List<Sphere> spheres, Lighting lighting)
        {
            _spheres = spheres;
            _lighting = lighting;
        }

        public Ray CreateEyeRay(Vector2 pixel)
        {
            Vector3 f = Vector3.Normalize(_lookAt - _eye);
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, _up));
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));
            Vector3 d = f + pixel.X * r * (float)Math.Tan(_FOV / 2) + pixel.Y * u * (float)Math.Tan(_FOV / 2);

            return new Ray(_eye, Vector3.Normalize(d));
        }

        public float FindHitPoint(Ray ray, Sphere sp)
        {
            var a = 1;
            var cE = ray._origin - sp._center;
            var b = 2 * Vector3.Dot(cE, ray._direction);
            var c = cE.Length() * cE.Length() - sp._radius * sp._radius;
            var discriminant = b * b - 4 * a * c;
            var determinant = Math.Sqrt(discriminant);

            if (discriminant >= 0)
            { 
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
            Vector3 H = Vector3.Zero;
            Vector3 colour = Vector3.Zero;
            Sphere closestSphere = null;
            float smallestLambda = float.PositiveInfinity;

            foreach(Sphere sp in _spheres)
            {
                float lambda = FindHitPoint(ray, sp);

                if (lambda > 0 && lambda < smallestLambda)
                {
                    H = ray._origin + lambda * ray._direction;
                    colour = sp._color;
                    smallestLambda = lambda;
                    closestSphere = sp;
                }
            }
            return new HitPoint(ray, H, colour, closestSphere);
        }

        public static Vector3 GetDiffuseLight(float nL, Vector3 lightColor, Vector3 sphereColor)
        {
            Vector3 diffLight = Vector3.Zero;
            if (nL >= 0)
            {
                diffLight = lightColor * sphereColor * nL;
            }

            return diffLight;
        }

        public static Vector3 GetSpecularLight(float nL, Vector3 lightColor, Vector3 sphereColor, Vector3 r, Vector3 EH)
        {
            Vector3 specularLight = Vector3.Zero;
            if (nL >= 0)
            {
                float phongFactor = (float)Math.Pow(Math.Max(0, Vector3.Dot(r, EH)), _k);
                specularLight = sphereColor + lightColor * phongFactor;
            }

            return specularLight;
        }

        public Vector3 GetShadowLight(Light light, Vector3 sphereColor, HitPoint hitPoint)
        {
            Vector3 shadowLight = Vector3.Zero;
            Ray lightRay = new Ray(hitPoint._h, Vector3.Normalize(light._position - hitPoint._h));
            lightRay._origin += lightRay._direction * 0.001f;
            HitPoint shadow = FindClosestHitPoint(lightRay);

            if (shadow != null && (shadow._h - hitPoint._h).Length() < (light._position - hitPoint._h).Length())
            {
                shadowLight = light._color * sphereColor;
            }

            return shadowLight;
        }
        

        public Vector3 CalcColour(CheckBoxControl checkBoxControl, Ray ray, int reflectionFactor = 0)
        {            
            HitPoint hitpoint = FindClosestHitPoint(ray);
            Vector3 color = Vector3.Zero;

            // No hitpoint found
            if (hitpoint == null || hitpoint._sphere == null){ return Vector3.Zero;}

            foreach (Light light in _lighting._lights)
            {
                // Get overall settings
                Vector3 n = Vector3.Normalize(hitpoint._h - hitpoint._sphere._center);     // Normal vector of hitpoint
                Vector3 l = Vector3.Normalize(Vector3.Subtract(light._position, hitpoint._h));
                float nL = Vector3.Dot(n, l);
                Vector3 s = l - Vector3.Dot(l, n) * n;
                Vector3 EH = Vector3.Normalize(Vector3.Subtract(_eye, hitpoint._h));
                Vector3 r = Vector3.Normalize(l - 2 * s);

                // Case 1: Simple Ray Tracing 
                color = hitpoint._color;               

                // Case 2: Diffuse/Lambert Light
                if (checkBoxControl.IsDiffuseCheckBoxChecked) { color = GetDiffuseLight(nL, light._color, color); }

                // Case 3: Phong/Specular
                if (checkBoxControl.IsSpecularCheckBoxChecked) { color = GetSpecularLight(nL, light._color, color, r, EH); }

                // Case 4: Shadows
                if (checkBoxControl.IsShadowCheckBoxChecked) { color -= GetShadowLight(light, color, hitpoint); }

                // Case 6: Procedural Textures
                if (checkBoxControl.IsProceduralTextureCheckBoxChecked && hitpoint._sphere._proceduralTexture){ color = ProceduralTexturing.GetProceduralColor(n.X, n.Y, n.Z);}

                // Case 7: Bitmap textures
                if (checkBoxControl.IsBitmapTextureCheckBoxChecked && hitpoint._sphere._bitmapTexture) { color = bitmapTexturing.GetBitmapColor(n.X, n.Y, n.Z); }
            }
            // Case 5: Reflections
            if (checkBoxControl.IsReflectionCheckBoxChecked)
                {
                    if (hitpoint._sphere._reflection && _reflectionStep < 10)
                    {
                        Vector3 l1 = Vector3.Normalize(_eye - hitpoint._h);
                        Vector3 n2 = Vector3.Normalize(hitpoint._h - hitpoint._sphere._center);
                        Vector3 r2 = 2 * (Vector3.Dot(l1, n2)) * n2 - l1;

                        Vector3 col = CalcColour(checkBoxControl, new Ray(_eye, r2), _reflectionStep + 1);

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
