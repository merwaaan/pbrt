using pbrt.bxdfs;
using pbrt.core;

namespace pbrt.materials
{
    class PlasticMaterial : Material
    {
        private readonly Spectrum Diffuse;
        private readonly Spectrum Specular;
        private readonly float Roughness;

        public PlasticMaterial(Spectrum diffuse, Spectrum specular, float roughness)
        {
            Diffuse = diffuse;
            Specular = specular;
            Roughness = roughness;
        }

        public void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            inter.Bsdf = new BSDF(inter);

            if (!Diffuse.IsBlack())
            {
                inter.Bsdf.Add(new LambertianReflection(Diffuse));
            }

            if (!Specular.IsBlack())
            {
                /*var fresnel = new FresnelDielectric(1.0f, 1.5f);
                var distribution = new TrowbridgeReitzDistribution(Roughness, Roughness);
                inter.Bsdf.Add(new MicrofacetsReflection(Specular, distribution, fresnel));*/
            }
        }
    }
}
