using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.bxdfs
{
    class MicrofacetReflection : BxDF
    {
        private Spectrum reflectance;
        private MicrofacetDistribution distribution;
        private Fresnel fresnel;

        public MicrofacetReflection(Spectrum r, MicrofacetDistribution d, Fresnel f)
            : base(BxDFType.Reflection | BxDFType.Specular) // TODO initially Glossy, not sure about the diff
        {
            reflectance = r;
            distribution = d;
            fresnel = f;
        }

        public override Spectrum f(Vector3<float> wo, Vector3<float> wi)
        {
            var cosThetaO = AbsCosTheta(wo);
            var cosThetaI = AbsCosTheta(wi);

            var wh = wi + wo;

            // Handle degenerate cases at grazing angles
            if (cosThetaI == 0 || cosThetaO == 0)
                return Spectrum.Zero;
            if (wh.LengthSquared() == 0)
                return Spectrum.Zero;

            wh = wh.Normalized();

            var f = fresnel.Evaluate(wi.Dot(wh));

            // Torrance-Sparrow model
            return
                (reflectance * distribution.D(wh) * distribution.G(wo, wi) * f) /
                (4 * cosThetaI * cosThetaO);
        }

        public override Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> u, out float pdf, out BxDFType sampledType)
        {
            pdf = 0;
            sampledType = 0;

            // Sample microfacet orientation and compute the incoming direction
            var wh = distribution.Sample_wh(wo, u);
            wi = Reflect(wo, wh);

            // If the reflected direction is in the opposite hemisphere, no light is reflected
            if (!MathUtils.SameHemisphere(wo, wi))
                return Spectrum.Zero;

            // Transform the PDF (Pdf() gives the distribution around the half-angle vector
            // but the integral requires the distribution around the incoming direction)
            pdf = distribution.Pdf(wo, wh) / (4 * wo.Dot(wh));

            return f(wo, wi);
        }

        public override float Pdf(Vector3<float> wo, Vector3<float> wi)
        {
            if (!MathUtils.SameHemisphere(wo, wi))
                return 0;

            var wh = (wo + wi).Normalized();
            return distribution.Pdf(wo, wh) / (4 * wo.Dot(wh));
        }
    }
}
