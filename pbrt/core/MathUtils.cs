using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    static class MathUtils
    {
        public static float Pi => (float)Math.PI;
        public static float InvPi => (float)(1.0f / Math.PI);
        public static float PiOver2 => (float)(Math.PI / 2.0f);
        public static float PiOver4 => (float)(Math.PI / 4.0f);

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

        public static bool SameHemisphere(Vector3<float> w, Vector3<float> wp)
        {
            return w.Z * wp.Z > 0;
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
    }
}
