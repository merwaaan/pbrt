using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    static class MathUtils
    {
        public const float Pi = 3.1415926535897f;
        public const float InvPi = 0.31830988618f;
        public const float Inv2Pi = 0.15915494309f;
        public const float PiOver2 = 1.57079632679f;
        public const float PiOver4 = 0.78539816339f;
        public const float TwoPi = 6.28318530718f;

        public static float Clamp(float x, float min, float max)
        {
            return (float)(Math.Max(min, Math.Min(max, x)));
        }

        public static float Radians(float deg)
        {
            return (Pi / 180) * deg;
        }

        public static float Degrees(float rad)
        {
            return (180 / Pi) * rad;
        }

        public static Vector3<float> CosineSampleHemisphere(Point2<float> u)
        {
            var d = ConcentricSampleDisk(u);
            var z = (float)Math.Sqrt(Math.Max(0, 1 - d.X * d.X - d.Y * d.Y));
            return new Vector3<float>(d.X, d.Y, z);
        }

        public static Point2<float> ConcentricSampleDisk(Point2<float> u)
        {
            // Map uniform random numbers to $[-1,1]^2$
            var uOffset = u * 2.0f - Point2<float>.One;

            // Handle degeneracy at the origin
            if (uOffset.X == 0 && uOffset.Y == 0) return new Point2<float>(0, 0);

            // Apply concentric mapping to point
            float theta, r;
            if (Math.Abs(uOffset.X) > Math.Abs(uOffset.Y))
            {
                r = uOffset.X;
                theta = PiOver4 * (uOffset.Y / uOffset.X);
            }
            else
            {
                r = uOffset.Y;
                theta = PiOver2 - PiOver4 * (uOffset.X / uOffset.Y);
            }

            return new Point2<float>((float)Math.Cos(theta), (float)Math.Sin(theta)) * r;
        }

        public static bool Quadratic(float a, float b, float c, out float t0, out float t1)
        {
            t0 = t1 = 0.0f;

            float delta = b * b - 4 * a * c;
            if (delta < 0)
                return false;

            if (delta == 0.0f)
            {
                t0 = -b / (2 * a);
            }
            else
            {
                var sqrt = (float)Math.Sqrt(delta);
                t0 = (-b + sqrt) / (2 * a);
                t1 = (-b - sqrt) / (2 * a);

                if (t1 < t0)
                {
                    var tmp = t1;
                    t1 = t0;
                    t0 = tmp;
                }
            }

            return true;
        }

        public static Vector3<float> UniformSampleHemisphere(Point2<float> u)
        {
            var z = u.X;
            var r = Math.Sqrt(Math.Max(0, 1 - z * z));
            var phi = 2 * Pi * u.Y;
            return new Vector3<float>((float)(r * Math.Cos(phi)), (float)(r * Math.Sin(phi)), z);
        }

        public static float UniformHemispherePdf()
        {
            return Inv2Pi;
        }

        public static bool SameHemisphere(Vector3<float> w, Vector3<float> wp)
        {
            return w.Z * wp.Z > 0;
        }

        public static Vector3<float> SphericalDirection(float sinTheta, float cosTheta, float phi)
        {
            return new Vector3<float>(sinTheta * (float)Math.Cos(phi), sinTheta * (float)Math.Sin(phi), cosTheta);
        }
        public static float PowerHeuristic(int nf, float fPdf, int ng, float gPdf)
        {
            var f = nf * fPdf;
            var g = ng * gPdf;
            return (f * f) / (f * f + g * g);
        }
    }
}
