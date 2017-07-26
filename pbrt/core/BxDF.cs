using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public abstract class BxDF
    {
        public enum BxDFType
        {
            Reflection = 1,
            Transmission = 2,
            Diffuse = 4,
            Glossy = 8,
            Specular = 16,
            All = Reflection | Transmission | Diffuse | Glossy | Specular
        }

        public readonly BxDFType Type;

        public BxDF(BxDFType type)
        {
            Type = type;
        }

        public bool Matches(BxDFType type)
        {
            return (Type & type) > 0;
        }

        // Computes the distribution for the given pair of directions
        public abstract Spectrum f(Vector3<float> wo, Vector3<float> wi);

        // Sample an incoming direction for the given outgoing direction
        public virtual Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> sample, out float pdf, out BxDFType type)
        {
            wi = CosineSampleHemisphere(sample);

            if (wo.Z < 0)
                wi.Z *= -1;

            pdf = 0;// TODO !SameHemisphere(wo, wi) ? AbsCosTheta(wi) * MathUtils.InvPi : 0;
            type = 0;

            return f(wo, wi);
        }

        // TODO move to sampling
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
                theta = MathUtils.PiOver4 * (uOffset.Y / uOffset.X);
            }
            else
            {
                r = uOffset.Y;
                theta = MathUtils.PiOver2 - MathUtils.PiOver4 * (uOffset.X / uOffset.Y);
            }
            return new Point2<float>((float)Math.Cos(theta), (float)Math.Sin(theta)) * r;
        }

        // Hemispherical-directional reflectance: total reflection in the given direction
        // due to constant illumination over the hemisphere
        //public abstract Spectrum rho(Vector3<float> wo, int sampleCount, out Point2<float> samples);

        // Hemispherical-hemispherical reflectance: fraction of incident light reflected
        // when the incident light is the same from all directions
        //public abstract Spectrum rho(int sampleCount, out Point2<float> sample1, out Point2<float> sample2);

        public static Vector3<float> Reflect(Vector3<float> wo, Vector3<float> n)
        {
            return wo.Negated() + n * Vector3<float>.Dot(wo, n) * 2;
        }

        public static float CosTheta(Vector3<float> w)
        {
            // Because we are in the BRDF coordinate frame:
            // cos(theta) = n . w = (0, 0, 1) . w = w.z
            return w.Z;
        }

        public static float Cos2Theta(Vector3<float> w)
        {
            return w.Z * w.Z;
        }

        public static float AbsCosTheta(Vector3<float> w)
        {
            return Math.Abs(w.Z);
        }

        public static float SinTheta(Vector3<float> w)
        {
            return (float)Math.Sqrt(Sin2Theta(w));
        }

        public static float Sin2Theta(Vector3<float> w)
        {
            // Because sin² + cos² = 1
            return Math.Max(0, 1 - Cos2Theta(w));
        }

        public static float TanTheta(Vector3<float> w)
        {
            return SinTheta(w) / CosTheta(w);
        }

        public static float Tan2Theta(Vector3<float> w)
        {
            return Sin2Theta(w) / Cos2Theta(w);
        }

        public static float CosPhi(Vector3<float> w)
        {
            var sinTheta = SinTheta(w);
            return sinTheta == 0 ? 1 : MathUtils.Clamp(w.X / sinTheta, -1, 1);
        }

        public static float Cos2Phi(Vector3<float> w)
        {
            var cosPhi = CosPhi(w);
            return cosPhi * cosPhi;
        }

        public static float SinPhi(Vector3<float> w)
        {
            var sinTheta = SinTheta(w);
            return sinTheta == 0 ? 0 : MathUtils.Clamp(w.Y / sinTheta, -1, 1);
        }

        public static float Sin2Phi(Vector3<float> w)
        {
            var sinPhi = SinPhi(w);
            return sinPhi * sinPhi;
        }
    }
}
