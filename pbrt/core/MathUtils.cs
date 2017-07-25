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
            return (float)(Math.Max(0, Math.Min(1, x)));
        }

        public static float Radians(float deg)
        {
            return (Pi / 180) * deg;
        }

        public static float Degrees(float rad)
        {
            return (180 / Pi) * rad;
        }
    }
}
