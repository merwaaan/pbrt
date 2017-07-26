using pbrt.bxdfs;
using pbrt.core;

namespace pbrt.materials
{
    class PlasticMaterial : Material
    {
        private readonly Spectrum diffuse;
        private readonly Spectrum specular;
        private readonly float roughness;

        public PlasticMaterial(Spectrum d, Spectrum s, float r)
        {
            diffuse = d;
            specular = s;
            roughness = r;
        }

        public void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            inter.Bsdf = new BSDF(inter);

            if (!diffuse.IsBlack())
            {
                inter.Bsdf.Add(new LambertianReflection(diffuse));
            }

            if (!specular.IsBlack())
            {
                var fresnel = new FresnelDielectric(1.5f, 1.0f);
                var distribution = new TrowbridgeReitzDistribution(roughness);
                inter.Bsdf.Add(new MicrofacetReflection(specular, distribution, fresnel));
            }
        }
    }
}
