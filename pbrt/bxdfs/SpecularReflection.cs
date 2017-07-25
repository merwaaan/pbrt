using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.bxdfs
{
    public class SpecularReflection : BxDF
    {
        private Spectrum reflectance;
        private Fresnel fresnel;

        public SpecularReflection(Spectrum r, Fresnel fresnel)
            : base(BxDFType.Reflection | BxDFType.Specular)
        {
            this.reflectance = r;
            this.fresnel = fresnel;
        }

        public override Spectrum f(Vector3<float> wo, Vector3<float> wi)
        {
            // No scattering
            return Spectrum.Zero;
        }

        public override Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> sample, out float pdf, out BxDFType type)
        {
            // Compute the perfect specular reflection direction
            //
            // In polar coordinates:
            //   phi_o = phi_i + pi
            //   theta_o = theta_i
            wi = new Vector3<float>(-wo.X, -wo.Y, wo.Z); // Simplified because we are in the BRDF coordinate frame

            pdf = 1;
            sample = null;
            type = 0; // TODO?
            return fresnel.Evaluate(CosTheta(wi)) * reflectance * (1.0f / AbsCosTheta(wi));
        }

        /*public override Spectrum rho(Vector3<float> wo, int sampleCount, out Point2<float> samples)
        {

        }

        public override Spectrum rho(int sampleCount, out Point2<float> sample1, out Point2<float> sample2)
        {
            throw new NotImplementedException();
        }*/
    }
}
