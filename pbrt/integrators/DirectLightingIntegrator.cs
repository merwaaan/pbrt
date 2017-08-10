using pbrt.core;
using pbrt.core.geometry;
using System;
using System.Linq;
using static pbrt.core.BxDF;

namespace pbrt.integrators
{
    public class DirectLightingIntegrator : SamplerIntegrator
    {
        public enum LightStrategy
        {
            SampleOne, // Take a single sample from a single light
            SampleAll // Take multiple samples from all lights
        }

        private LightStrategy strategy = LightStrategy.SampleOne;
        private int maxDepth;

        private int[] lightSampleCounts;

        public DirectLightingIntegrator(Sampler sampler, int depth = 5)
            : base(sampler)
        {
            maxDepth = depth;
        }

        public override void Preprocess(Scene scene)
        {
            // Prepare samples for each light

            lightSampleCounts = scene.Lights
                .Select(light => Sampler.RoundCount(light.SampleCount)) // RoundCount might change the sample count
                .ToArray();

            if (strategy == LightStrategy.SampleAll)
            {
                for (var depth = 0; depth < maxDepth; ++depth)
                    for (var l = 0; l < scene.Lights.Count; ++l)
                    {
                        Sampler.Request2DArray(lightSampleCounts[l]); // For a position on the light
                        Sampler.Request2DArray(lightSampleCounts[l]); // For a scaterring direction from the BSDF
                    }
            }
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, Camera camera, Sampler sampler, int depth = 0)
        {
            var L = Spectrum.Zero;

            // If the ray doesn't intersect the scene, return the background radiance
            if (!scene.Intersect(ray, out SurfaceInteraction inter) || inter == null)
            {
                foreach (var light in scene.Lights)
                    L += light.Le(ray);
                //return new Spectrum(0, 0, 1);
                return L;
            }

            // If the ray intersects something, compute how the light is scattered            
            inter.ComputeScatteringFunctions(ray);

            // Add the emissive contribution of the intersected object
            L += inter.Le(inter.Wo);

            if (inter.Bsdf == null)
                return L;

            // Add the direct lighting contribution
            if (scene.Lights.Any())
            {
                if (strategy == LightStrategy.SampleAll)
                    L += UniformSampleAllLights(inter, scene, sampler);
                else
                    L += UniformSampleOneLight(inter, scene, sampler);
            }

            // Recursively trace new rays
            if (depth + 1 < maxDepth)
            {
                L += SpecularReflect(ray, inter, scene, camera, sampler, depth);
                //L += SpecularTransmit(ray, inter, scene, sampler, depth);
            }

            return L;
        }

        public Spectrum UniformSampleAllLights(Interaction inter, Scene scene, Sampler sampler)
        {
            var L = Spectrum.Zero;

            for (var l = 0; l < scene.Lights.Count; ++l)
            {
                var light = scene.Lights[l];

                // Estimate direct lighting using the requested sample arrays

                var lightPointArray = sampler.Get2DArray(lightSampleCounts[l]);
                var scatteringArray = sampler.Get2DArray(lightSampleCounts[l]);

                if (lightPointArray != null && scatteringArray != null)
                {
                    var Ld = Spectrum.Zero;
                    for (var sample = 0; sample < light.SampleCount; ++sample)
                        Ld += EstimateDirect(inter, scatteringArray[sample], light, lightPointArray[sample], scene, sampler);
                    L += Ld / light.SampleCount;
                }

                // If not available, fall back to single sample estimates

                else
                {
                    var lightPoint = sampler.Get2D();
                    var scattering = sampler.Get2D();
                    L += EstimateDirect(inter, scattering, light, lightPoint, scene, sampler);
                }

            }

            return L;
        }

        public Spectrum UniformSampleOneLight(Interaction inter, Scene scene, Sampler sampler)
        {
            if (!scene.Lights.Any())
                return Spectrum.Zero;

            // Randomly choose a light
            var light = scene.Lights[Window.Random.Next(scene.Lights.Count)];

            // Compensate sampling a single light by multiplying the result by the light count
            // (will average the contribution of each light over enough samples)
            var lightPoint = sampler.Get2D();
            var scattering = sampler.Get2D();
            return EstimateDirect(inter, scattering, light, lightPoint, scene, sampler) * scene.Lights.Count;
        }

        public static Spectrum EstimateDirect(Interaction inter, Point2<float> scattering, Light light, Point2<float> lightPoint, Scene scene, Sampler sampler, bool specular = false)
        {
            var bsdfType = specular ? BxDFType.All : BxDFType.All & ~BxDFType.Specular;

            var Ld = Spectrum.Zero;
            float scatteringPdf, lightPdf;

            // Sampling the BSDF works well with highly specular materials and large light sources.
            // Sampling the light source works well with small sources and rough materials.
            //
            // To handle most cases, we do both:
            //   1. Sample the light source (gives a radiance, a PDF, an incoming direction)
            //        -> evaluate the BSDF for those parameters
            //   2. Sample the BSDF
            //        -> compute the radiance along the incoming direction

            // 1. Sample the light source

            var Li = light.Sample_Li(inter, lightPoint, out Vector3<float> wi, out lightPdf, out VisibilityTester visibility);

            if (lightPdf > 0 && !Li.IsBlack())
            {
                // Evaluate the BSDF value for the light sample
                Spectrum f;
                if (inter.IsSurfaceInteraction())
                {
                    var sinter = (SurfaceInteraction)inter;
                    f = sinter.Bsdf.f(sinter.Wo, wi, bsdfType) * Vector3<float>.AbsDot(wi, sinter.Shading.N);
                    scatteringPdf = sinter.Bsdf.Pdf(sinter.Wo, wi, bsdfType);
                }
                else
                {
                    // Medium interaction
                    throw new NotImplementedException();
                }

                if (!f.IsBlack() && visibility.Unoccluded(scene))
                {
                    // Delta lights: do not apply multiple importance sampling
                    if (light.IsDeltaLight())
                    {
                        Ld += f * Li / lightPdf;
                    }
                    else
                    {
                        var weight = MathUtils.PowerHeuristic(1, lightPdf, 1, scatteringPdf);
                        Ld += f * Li * weight / lightPdf;
                    }
                }
            }

            // 2. Sample the BSDF

            if (!light.IsDeltaLight())
            {
                var sampledSpecular = false;

                // Sample scattered direction
                Spectrum f;
                if (inter.IsSurfaceInteraction())
                {
                    var sinter = (SurfaceInteraction)inter;
                    f = sinter.Bsdf.Sample_f(sinter.Wo, out wi, scattering, out scatteringPdf, bsdfType, out BxDFType sampledType);
                    f *= Vector3<float>.AbsDot(wi, sinter.Shading.N);
                    sampledSpecular = (sampledType & BxDFType.Specular) > 0;
                }
                else
                {
                    // Medium interaction
                    throw new NotImplementedException();
                }

                if (!f.IsBlack() && scatteringPdf > 0)
                {
                    var weight = 1f;
                    if (!sampledSpecular)
                    {
                        lightPdf = light.Pdf_Li(inter, wi);
                        if (lightPdf == 0)
                            return Ld;

                        weight = MathUtils.PowerHeuristic(1, scatteringPdf, 1, lightPdf);
                    }

                    // Transmittance would be done here
                    var Tr = Spectrum.One;

                    var ray = inter.SpawnRay(wi);
                    var foundSurfaceInteraction = scene.Intersect(ray, out SurfaceInteraction lightInter);

                    // Add light contribution
                    Li = Spectrum.Zero;
                    if (foundSurfaceInteraction)
                    {
                        if (lightInter.Primitive.GetAreaLight() == light)
                            Li = lightInter.Le(-wi);
                    }
                    else
                    {
                        //Li = light.Le(ray);
                    }

                    if (!Li.IsBlack())
                        Ld += f * Li * Tr * weight / scatteringPdf;
                }
            }

            return Ld;
        }

        private Spectrum SpecularReflect(RayDifferential ray, SurfaceInteraction inter, Scene scene, Camera camera, Sampler sampler, int depth)
        {
            // Sample a direction with the BSDF
            var type = BxDFType.Reflection | BxDFType.Specular;
            var f = inter.Bsdf.Sample_f(inter.Wo, out Vector3<float> wi, sampler.Get2D(), out float pdf, type, out BxDF.BxDFType sampledType);

            // Add the contribution of this reflection
            if (pdf > 0 && !f.IsBlack() && Vector3<float>.AbsDot(wi, inter.Shading.N) != 0)
            {
                var newRay = inter.SpawnRay(wi).ToDiff();

                if (ray.HasDifferentials)
                {
                    // TODO
                }

                return f * Li(newRay, scene, camera, sampler, depth + 1) * Vector3<float>.AbsDot(wi, inter.Shading.N) * (1.0f / pdf);
            }

            return Spectrum.Zero;
        }

        public override string ToString()
        {
            return $"DirectLighting (depth {maxDepth}, samples {Sampler.SamplesPerPixel})";
        }
    }
}
