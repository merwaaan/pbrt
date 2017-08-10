using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public abstract class MicrofacetDistribution
    {
        // Sample the distribution of visible microfacets OR the overall distribution.
        //
        // Sampling the overall distribution only considers the D term of a microfacet BSDF (normal distribution).
        // Sampling the visible area is more accurate as it also considers the geometric terms (occlusion, self-shadowing).
        protected bool sampleVisibleArea;

        protected MicrofacetDistribution(bool sampleVisible)
        {
            sampleVisibleArea = sampleVisible;
        }

        // Gives the ratio of microfacets that are aligned with the given normal vector
        public abstract float D(Vector3<float> wh);

        // Gives the ratio of masked microfacet area per microfacet area
        public abstract float Lambda(Vector3<float> w);

        // Gives the ratio of microfacets that are visible from direction w
        public float G1(Vector3<float> w)
        {
            return 1.0f / (1 + Lambda(w));
        }

        public float G(Vector3<float> wo, Vector3<float> wi)
        {
            return 1.0f / (1 + Lambda(wo) + Lambda(wi));
        }

        // Sample a microfacet orientation from this distribution given an outgoing direction
        public abstract Vector3<float> Sample_wh(Vector3<float> wo, Point2<float> u);

        //
        public float Pdf(Vector3<float> wo, Vector3<float> wh)
        {
            if (sampleVisibleArea)
            {
                return D(wh) * G1(wo) * Vector3<float>.AbsDot(wo, wh) / BxDF.AbsCosTheta(wo);
            }
            else
            {
                return D(wh) * BxDF.AbsCosTheta(wo);
            }
        }
    }

    public class BeckmannDistribution : MicrofacetDistribution
    {
        private float alphaX, alphaY;

        public BeckmannDistribution(float ax, float ay, bool sampleVisible = true)
            : base(sampleVisible)
        {
            alphaX = ax;
            alphaY = ay;
        }

        public BeckmannDistribution(float a, bool sampleVisible = true)
            : this(a, a, sampleVisible)
        {
        }

        public override float D(Vector3<float> wh)
        {
            var tan2Theta = BxDF.Tan2Theta(wh);

            // Special case: tan² converges to infinity (at grazing angles)
            if (float.IsInfinity(tan2Theta))
                return 0;

            var cos2Theta = BxDF.Cos2Theta(wh);

            return
                (float)Math.Exp(-tan2Theta * (BxDF.Cos2Phi(wh) / (alphaX * alphaX) + BxDF.Sin2Phi(wh) / (alphaY * alphaY))) /
                (MathUtils.Pi * alphaX * alphaY * cos2Theta * cos2Theta);
        }

        public override float Lambda(Vector3<float> w)
        {
            var absTanTheta = Math.Abs(BxDF.TanTheta(w));

            if (float.IsInfinity(absTanTheta))
                return 0;

            var alpha = (float)Math.Sqrt(BxDF.Cos2Phi(w) * alphaX * alphaX + BxDF.Sin2Phi(w) * alphaY * alphaY);
            var a = 1.0f / (alpha / absTanTheta);
            if (a >= 1.6f)
                return 0;

            return (1 - 1.259f * a + 0.396f * a * a) / (3.535f * a + 2.181f * a * a);
        }

        public override Vector3<float> Sample_wh(Vector3<float> wo, Point2<float> u)
        {
            // Sample the full distribution of normals
            if (!sampleVisibleArea)
            {
                float tan2Theta, phi;

                // Isotropic distribution
                if (alphaX == alphaY)
                {
                    var logSample = (float)Math.Log(u.X);
                    if (logSample == float.PositiveInfinity)
                        logSample = 0;

                    tan2Theta = -alphaX * alphaX * logSample;
                    phi = u.Y * 2 * MathUtils.Pi;
                }
                // Anisotropic distribution
                else
                {
                    throw new NotImplementedException();
                }

                // Map sampled angles to a normal direction
                var cosTheta = 1 / (float)Math.Sqrt(1 + tan2Theta);
                var sinTheta = (float)Math.Sqrt(Math.Max(0, 1 - cosTheta * cosTheta));

                var wh = MathUtils.SphericalDirection(sinTheta, cosTheta, phi);
                if (!MathUtils.SameHemisphere(wo, wh))
                    wh = -wh;

                return wh;
            }
            // Sample the visible area of normals
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public class TrowbridgeReitzDistribution : MicrofacetDistribution
    {
        private float alphaX, alphaY;

        public TrowbridgeReitzDistribution(float a, bool sampleVisible = true)
            : this(a, a, sampleVisible)
        {
        }

        public TrowbridgeReitzDistribution(float ax, float ay, bool sampleVisible = true)
            : base(sampleVisible)
        {
            alphaX = ax;
            alphaY = ay;
        }

        public override float D(Vector3<float> wh)
        {
            var tan2Theta = BxDF.Tan2Theta(wh);

            // Special case: tan² converges to infinity (at grazing angles)
            if (float.IsInfinity(tan2Theta))
                return 0;

            var cos2Theta = BxDF.Cos2Theta(wh);

            var e = (BxDF.Cos2Phi(wh) / (alphaX * alphaX) + BxDF.Sin2Phi(wh) / (alphaY * alphaY)) * tan2Theta;
            return 1.0f / (MathUtils.Pi * alphaX * alphaY * cos2Theta * cos2Theta * (1 + e) * (1 + e));
        }

        public override float Lambda(Vector3<float> w)
        {
            var absTanTheta = Math.Abs(BxDF.TanTheta(w));

            if (float.IsInfinity(absTanTheta))
                return 0;

            var alpha = (float)Math.Sqrt(BxDF.Cos2Phi(w) * alphaX * alphaX + BxDF.Sin2Phi(w) * alphaY * alphaY);
            var alpha2Tan2Theta = (alpha * absTanTheta) * (alpha * absTanTheta);
            return (-1 + (float)Math.Sqrt(1 + alpha2Tan2Theta)) / 2.0f;
        }

        public override Vector3<float> Sample_wh(Vector3<float> wo, Point2<float> u)
        {
            throw new NotImplementedException();
        }
    }
}
