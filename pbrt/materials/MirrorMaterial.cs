using pbrt.bxdfs;
using pbrt.core;

namespace pbrt.materials
{
    class MirrorMaterial : Material
    {
        private readonly Spectrum Reflectance;

        public MirrorMaterial(Spectrum reflection)
        {
            Reflectance = reflection;
        }

        public void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            inter.Bsdf = new BSDF(inter);

            if (!Reflectance.IsBlack())
            {
                inter.Bsdf.Add(new SpecularReflection(Reflectance, new FresnelNoOp()));
            }
        }
    }
}
