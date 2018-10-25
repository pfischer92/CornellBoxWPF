using CornellBoxWPF.BitmapHelper;
using CornellBoxWPF.Light;
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
        public bool _bhvEnabled = false;

        private List<Sphere> _spheres = new List<Sphere>();
        private Lighting _lighting = new Lighting();
        BitmapTexturing bitmapTexturing = new BitmapTexturing();
        public static Random random = new Random();

        public Scene(List<Sphere> spheres, Lighting lighting, bool bhvEnabled = false)
        {
            _spheres = spheres;
            _lighting = lighting;
            _bhvEnabled = bhvEnabled;
        }

        public Ray CreateEyeRay(Vector2 pixel)
        {
            Vector3 f = Vector3.Normalize(_lookAt - _eye);
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, _up));
            Vector3 u = Vector3.Normalize(Vector3.Cross(r, f));
            Vector3 d = f + pixel.X * r * (float)Math.Tan(_FOV / 2) + pixel.Y * u * (float)Math.Tan(_FOV / 2);

            return new Ray(_eye, Vector3.Normalize(d));
        }

        public float GetRandomNumber(float minimum, float maximum)
        {
            return (float)random.NextDouble() * (maximum - minimum) + minimum;
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
            if (_bhvEnabled)
            {
                // Todo: Traverse through tree structure
                return null;
            }
            else
            {
                Vector3 H = Vector3.Zero;
                Vector3 colour = Vector3.Zero;
                Sphere closestSphere = null;
                float smallestLambda = float.PositiveInfinity;

                foreach (Sphere sp in _spheres)
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

        public static Vector3 GetSpecularLight(float nL, HitPoint hitPoint, Vector3 lightColor, Vector3 r, Vector3 EH)
        {
            Vector3 specularLight = Vector3.Zero;
            if (nL >= 0)
            {
                float phongFactor = Vector3.Dot(r, Vector3.Normalize(hitPoint._point - _eye));
                specularLight = lightColor * (float)Math.Pow(phongFactor, _k);
            }

            return specularLight;
        }

        public Vector3 GetShadowLight(LightSource light, HitPoint hitPoint, Vector3 sphereColor)
        {
            Vector3 shadowLight = Vector3.Zero;
            Ray lightRay = new Ray(hitPoint._point, Vector3.Normalize(light._position - hitPoint._point));
            lightRay._origin += lightRay._direction * 0.001f;
            HitPoint shadow = FindClosestHitPoint(lightRay);

            if (shadow != null && (shadow._point - hitPoint._point).Length() < (light._position - hitPoint._point).Length())
            {
                shadowLight = light._color * sphereColor;
            }

            return shadowLight;
        }

        public Vector3 GetSoftShadowLight(LightSphere lp, HitPoint hitPoint, Vector3 sphereColor)
        {
            Vector3 shadowLight = Vector3.Zero;
            Vector3 NL = Vector3.Normalize(hitPoint._point - lp._position);
            Vector3 Nx = Vector3.Normalize(Vector3.Cross(NL, _up));
            Vector3 Ny = Vector3.Cross(NL, Nx);
            int numOfCalc = 20;
            int hits = 0;

            for (int i = 0; i < numOfCalc; i++)
            {
                Vector3 P = lp._position + lp._Radius * GetRandomNumber(0f, 1f) * Nx + Ny * GetRandomNumber(0f, 1f) * lp._Radius;
                Ray lightRay = new Ray(hitPoint._point, Vector3.Normalize(P - hitPoint._point));
                lightRay._origin += lightRay._direction * 0.001f;
                HitPoint shadowHitpoint = FindClosestHitPoint(lightRay);

                if (shadowHitpoint != null && (shadowHitpoint._point - hitPoint._point).Length() < (lp._position - hitPoint._point).Length())
                {
                    hits++;
                    shadowLight = lp._color * sphereColor * 0.7f;
                }
            }

            shadowLight = shadowLight * hits / numOfCalc;
            return shadowLight;
        }
        

        public Vector3 CalcColour(CheckBoxControl checkBoxControl, Ray ray, int reflectionFactor = 0)
        {            
            HitPoint hitpoint = FindClosestHitPoint(ray);
            Vector3 color = Vector3.Zero;
            Vector3 diffColor = Vector3.Zero;

            // No hitpoint found
            if (hitpoint == null || hitpoint._sphere == null){ return Vector3.Zero;}

            // Case 5: Reflections
            if (checkBoxControl.IsReflectionCheckBoxChecked)
            {
                if (hitpoint._sphere._reflection && _reflectionStep < 10)
                {
                    Vector3 l1 = Vector3.Normalize(_eye - hitpoint._point);
                    Vector3 n2 = Vector3.Normalize(hitpoint._point - hitpoint._sphere._center);
                    Vector3 r2 = 2 * (Vector3.Dot(l1, n2)) * n2 - l1;

                    Vector3 col = CalcColour(checkBoxControl, new Ray(_eye, r2), _reflectionStep + 1);

                    color += col;
                    return color;
                }
            }

            // Case 8: Soft shadows
            if (checkBoxControl.IsSoftShadowCheckBoxChecked)
            {
                LightSphere lp = new LightSphere(new Vector3(0.0f, -0.9f, 0), new Vector3(1.0f, 1.0f, 1.0f), 0.2f);

                Vector3 n = Vector3.Normalize(hitpoint._point - hitpoint._sphere._center);     
                Vector3 l = Vector3.Normalize(Vector3.Subtract(lp._position, hitpoint._point));
                float nL = Vector3.Dot(n, l);
                Vector3 s = l - Vector3.Dot(l, n) * n;
                Vector3 EH = Vector3.Normalize(Vector3.Subtract(_eye, hitpoint._point));
                Vector3 r = Vector3.Normalize(l - 2 * s);
                color += GetDiffuseLight(nL, lp._color, hitpoint._color);
                color += GetSpecularLight(nL, hitpoint, lp._color, r, EH);
                color -= GetSoftShadowLight(lp, hitpoint, hitpoint._color);
                return color;
            }

            foreach (LightSource light in _lighting._lights)
            {
                // Get overall settings
                Vector3 n = Vector3.Normalize(hitpoint._point - hitpoint._sphere._center); 
                Vector3 l = Vector3.Normalize(Vector3.Subtract(light._position, hitpoint._point));
                float nL = Vector3.Dot(n, l);
                Vector3 s = l - Vector3.Dot(l, n) * n;
                Vector3 EH = Vector3.Normalize(Vector3.Subtract(_eye, hitpoint._point));
                Vector3 r = Vector3.Normalize(l - 2 * s);

                // Case 1: Simple Ray Tracing 
                //color = hitpoint._color;

                // Case 2: Diffuse/Lambert Light
                if (checkBoxControl.IsDiffuseCheckBoxChecked) { color += GetDiffuseLight(nL, light._color, hitpoint._color); }

                // Case 3: Phong/Specular
                if (checkBoxControl.IsSpecularCheckBoxChecked) { color += GetSpecularLight(nL, hitpoint, light._color, r, EH); }                

                // Case 4: Shadows
                if (checkBoxControl.IsShadowCheckBoxChecked) { color -= GetShadowLight(light, hitpoint, color); }

                // Case 6: Procedural Textures
                if (checkBoxControl.IsProceduralTextureCheckBoxChecked && hitpoint._sphere._proceduralTexture){ color = ProceduralTexturing.GetProceduralColor(n.X, n.Y, n.Z);}

                // Case 7: Bitmap textures
                if (checkBoxControl.IsBitmapTextureCheckBoxChecked && hitpoint._sphere._bitmapTexture) { color = bitmapTexturing.GetBitmapColor(n.X, n.Y, n.Z); }
            }

            return color;
        }
    }
}
