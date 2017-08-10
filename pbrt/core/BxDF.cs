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
            return (Type & type) == Type;
        }

        // Computes the distribution for the given pair of directions
        public abstract Spectrum f(Vector3<float> wo, Vector3<float> wi);

        // Sample an incoming direction for the given outgoing direction
        public virtual Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> sample, out float pdf, out BxDFType type)
        {
            // The default implementation samples the hemisphere with a cosine-weighted distribution
            wi = MathUtils.CosineSampleHemisphere(sample);

            // Flip the incoming direction to be in the same hemisphere as the outgoing direction
            if (wo.Z < 0)
                wi.Z *= -1;

            // Probability Density Function
            //  - if not on the same hemisphere: 0
            //  - otherwise: wi . n
            pdf = Pdf(wo, wi);

            type = 0;

            return f(wo, wi);
        }

        public virtual float Pdf(Vector3<float> wo, Vector3<float> wi)
        {
            return MathUtils.SameHemisphere(wo, wi) ? AbsCosTheta(wi) * MathUtils.InvPi : 0;
        }

        // Hemispherical-directional reflectance: total reflection in the given direction
        // due to constant illumination over the hemisphere
        public Spectrum rho(Vector3<float> w, int sampleCount, Point2<float>[] u)
        {
            var r = Spectrum.Zero;

            for (var i = 0; i < sampleCount; ++i)
            {
                var f = Sample_f(w, out Vector3<float> wi, u[i], out float pdf, out BxDFType sampledType);
                if (pdf > 0)
                    r += f * AbsCosTheta(wi) / pdf;
            }

            return r / sampleCount;
        }

        // Hemispherical-hemispherical reflectance: fraction of incident light reflected
        // when the incident light is the same from all directions
        public Spectrum rho(int sampleCount, Point2<float>[] u1, Point2<float>[] u2)
        {
            var r = Spectrum.Zero;

            for (var i = 0; i < sampleCount; ++i)
            {
                var wo = MathUtils.UniformSampleHemisphere(u1[i]);
                var pdfo = MathUtils.UniformHemispherePdf();
                var f = Sample_f(wo, out Vector3<float> wi, u2[i], out float pdfi, out BxDFType sampledType);

                if (pdfi > 0)
                    r += f * AbsCosTheta(wi) * AbsCosTheta(wo) / (pdfo * pdfi);
            }

            return r / (MathUtils.Pi * sampleCount);
        }

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
