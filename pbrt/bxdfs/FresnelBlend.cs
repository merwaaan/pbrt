using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.bxdfs
{
    // Mixture of a diffuse and a glossy term
    class FresnelBlend : BxDF
    {
        private Spectrum rd, rs;
        private MicrofacetDistribution distribution;

        public FresnelBlend(Spectrum d, Spectrum s, MicrofacetDistribution distrib)
            : base(BxDFType.Reflection | BxDFType.Glossy)
        {
            rd = d;
            rs = s;
            distribution = distrib;
        }

        public override Spectrum f(Vector3<float> wo, Vector3<float> wi)
        {
            throw new NotImplementedException();
        }

        public override Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> u, out float pdf, out BxDFType sampledType)
        {
            pdf = 0;
            sampledType = 0;

            // Cosine-sample the hemisphere
            if (u.X < 0.5f)
            {
                u.X *= 2; // Remap the sample to avoid each path to have samples covering only half the range of possible value

                wi = MathUtils.CosineSampleHemisphere(u);

                if (wo.Z < 0)
                    wi.Z = -wi.Z;
            }
            // Sample the microfacet orientation
            else
            {
                u.X = 2 * (u.X - 0.5f); // Remap

                var wh = distribution.Sample_wh(wo, u);
                wi = Reflect(wo, wh);

                if (!MathUtils.SameHemisphere(wo, wi))
                    return Spectrum.Zero;
            }

            pdf = Pdf(wo, wi);
            return f(wo, wi);
        }

        public override float Pdf(Vector3<float> wo, Vector3<float> wi)
        {
            if (!MathUtils.SameHemisphere(wo, wi))
                return 0;

            var wh = (wo + wi).Normalized();
            var pdfWh = distribution.Pdf(wo, wh);
            
            return 0.5f * (AbsCosTheta(wi) * MathUtils.InvPi + pdfWh / (4 * wo.Dot(wh)));
        }
    }
}
