using pbrt.bxdfs;
using pbrt.core;

namespace pbrt.materials
{
    class MatteMaterial : Material
    {
        private readonly Spectrum Diffuse;
        private readonly float Roughness;

        public MatteMaterial(Spectrum diffuse, float roughness)
        {
           Diffuse = diffuse;
           Roughness = roughness;
        }

        public void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            var r = Diffuse.Clamp();
            //TODO sigma -> Oren-Nayar

            inter.Bsdf = new BSDF(inter);
            inter.Bsdf.Add(new LambertianReflection(r));
        }
    }
}
