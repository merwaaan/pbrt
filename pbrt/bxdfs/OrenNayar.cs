using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.bxdfs
{
    class OrenNayar : BxDF
    {
        private readonly Spectrum reflectance;
        private readonly float a, b;

        public OrenNayar(Spectrum r, float sigma)
            : base(BxDFType.Reflection | BxDFType.Diffuse)
        {
            reflectance = r;

            sigma = MathUtils.Radians(sigma);
            var sigma2 = sigma * sigma;

            a = 1 - (sigma2 / (2 * (sigma2 + 0.33f)));
            b = 0.45f * sigma2 / (sigma2 + 0.09f);
        }

        private const float invPi = (float)(1.0f / Math.PI);

        public override Spectrum f(Vector3<float> wo, Vector3<float> wi)
        {
            var sinThetaI = SinTheta(wi);
            var sinThetaO = SinTheta(wo);

            // Compute the cosine term
            float maxCos = 0;
            if (sinThetaI > 0.0001f && sinThetaO > 0.0001f)
            {
                float sinPhiI = SinPhi(wi), cosPhiI = CosPhi(wi);
                float sinPhiO = SinPhi(wo), cosPhiO = CosPhi(wo);
                var dCos = cosPhiI * cosPhiO + sinPhiI * sinPhiO; // cos(a - b) = cos(a) cos(b) + sin(a) sin(b)
                maxCos = Math.Max(0, dCos);
            }

            // Compute the sine and tangent terms
            float sinAlpha = 0, tanBeta = 0;
            if (AbsCosTheta(wi) > AbsCosTheta(wo))
            {
                sinAlpha = sinThetaO;
                tanBeta = sinThetaI / AbsCosTheta(wi);
            }
            else
            {
                sinAlpha = sinThetaI;
                tanBeta = sinThetaO / AbsCosTheta(wo);
            }

            return reflectance * MathUtils.InvPi * (a + b * maxCos * sinAlpha * tanBeta);
        }

        public override Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> sample, out float pdf, out BxDFType type)
        {
            throw new NotImplementedException();
        }
    }
}
