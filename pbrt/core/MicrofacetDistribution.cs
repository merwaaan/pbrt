using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public abstract class MicrofacetDistribution
    {
        // Gives the ratio of microfacets that are aligned with the given normal vector
        public abstract float D(Vector3<float> wh);

        // Gives the ratio of masked microfacet area per microfacet area
        public abstract float Lambda(Vector3<float> w);

        // Gives the ratio of microfacets that are visible from direction w.
        public float G1(Vector3<float> w)
        {
            return 1.0f / (1 + Lambda(w));
        }

        public float G(Vector3<float> wo, Vector3<float> wi)
        {
            return 1.0f / (1 + Lambda(wo) + Lambda(wi));
        }
    }

    public class BeckmannDistribution : MicrofacetDistribution
    {
        private float alphaX, alphaY;

        public BeckmannDistribution(float ax, float ay)
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
    }

    public class TrowbridgeReitzDistribution : MicrofacetDistribution
    {
        private float alphaX, alphaY;

        public TrowbridgeReitzDistribution(float a)
        {
            alphaX = alphaY = a;
        }

        public TrowbridgeReitzDistribution(float ax, float ay)
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
    }
}
