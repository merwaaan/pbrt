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
    }
}
