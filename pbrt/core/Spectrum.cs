using System;

namespace pbrt.core
{
    public struct Spectrum
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }

        public static Spectrum Zero => new Spectrum(0);
        public static Spectrum One => new Spectrum(1);

        public Spectrum(float x)
        {
            R = G = B = x;
        }

        public Spectrum(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool IsBlack()
        {
            return R == 0 && G == 0 && B == 0;
        }

        public bool HasNans()
        {
            return float.IsNaN(R) || float.IsNaN(G) || float.IsNaN(B);
        }

        public Spectrum Clamp(float low = 0, float high = float.PositiveInfinity)
        {
            return new Spectrum(
                Math.Max(low, Math.Min(high, R)),
                Math.Max(low, Math.Min(high, G)),
                Math.Max(low, Math.Min(high, B)));
        }

        public static Spectrum Lerp(Spectrum a, Spectrum b, float t)
        {
            return a * (1 - t) + b * t;
        }

        public static Spectrum operator +(Spectrum a, Spectrum b)
        {
            return new Spectrum(a.R + b.R, a.G + b.G, a.B + b.B);
        }

        public static Spectrum operator *(Spectrum a, Spectrum b)
        {
            return new Spectrum(a.R * b.R, a.G * b.G, a.B * b.B);
        }

        public static Spectrum operator *(Spectrum a, float s)
        {
            return new Spectrum(a.R * s, a.G * s, a.B * s);
        }

        public static Spectrum operator /(Spectrum a, float s)
        {
            return new Spectrum(a.R / s, a.G / s, a.B / s);
        }
    }
}
