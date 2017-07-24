using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.bxdfs
{
    class LambertianReflection : BxDF
    {
        // Reflectance spectrum: fraction of incident light that is scattered
        private Spectrum r;

        public LambertianReflection(Spectrum r)
            : base(BxDFType.Reflection | BxDFType.Diffuse)
        {
            this.r = r;
        }

        private const float invPi = (float)(1.0f / Math.PI);

        public override Spectrum f(Vector3<float> wo, Vector3<float> wi)
        {
            // Incident illumination is scattered equally in all directions
            return r * invPi;
        }

        public override Spectrum Sample_f(Vector3<float> wo, out Vector3<float> wi, Point2<float> sample, out float pdf, out BxDFType type)
        {
            throw new NotImplementedException();
        }
    }
}
