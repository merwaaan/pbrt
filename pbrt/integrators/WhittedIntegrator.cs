﻿using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.integrators
{
    public class WhittedIntegrator : SamplerIntegrator
    {
        private int _maxDepth;

        public WhittedIntegrator(Camera camera, Sampler sampler, int maxDepth = 5)
            : base(sampler, camera)
        {
            _maxDepth = maxDepth;
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, Sampler sampler, int depth = 0)
        {
            var L = Spectrum.Zero;

            // If the ray doesn't intersect the scene, return the background radiance
            if (!scene.Intersect(ray, out SurfaceInteraction inter))
            {
                foreach (var light in scene.Lights)
                    L += light.Le(ray);
                return L;
            }

            // If the ray intersects something, we need to compute how the light is scattered

            var n = inter.Shading.N;
            var wo = inter.Wo;

            inter.ComputeScatteringFunctions(ray);

            // Add the emissive contribution of the intersected object
            L += inter.Le(wo);

            // Add the direct contribution of each light source
            foreach (var light in scene.Lights)
            {
                // Sample the light source
                var Li = light.Sample_Li(inter, sampler.Get2D(), out Vector3<float> wi, out float pdf, out VisibilityTester visibility);

                if (Li.IsBlack() || pdf == 0)
                    continue;

                // Evaluate the scatteering at the interaction point
                var f = inter.Bsdf.f(wo, wi);

                // Trace a shadow ray to check that the point receives light
                if (!f.IsBlack())
                {
                    if (visibility.Unoccluded(scene))
                        L += f * Li * Vector3<float>.AbsDot(wi, n.ToVector3()) * (1.0f / pdf);
                    /*else
                        L = new Spectrum(0, 0, 50);*/
                }
            }

            // Recursively trace new rays
            if (depth + 1 < _maxDepth)
            {
                //L += SpecularReflect(ray, inter, scene, sampler, depth);
                //L += SpecularTransmit(ray, inter, scene, sampler, depth);
            }

            return L;
        }

        private Spectrum SpecularReflect(RayDifferential ray, SurfaceInteraction inter, Scene scene, Sampler sampler, int depth)
        {
            // Sample a direction with the BSDF
            /*var type = BxDF.BxDFType.Reflection | BxDF.BxDFType.Reflection;
            var f = inter.Bsdf.Sample_f(inter.Wo, out Vector3<float> wi, sampler.Get2D(), out float pdf, type);

            //
            if (pdf > 0 && !f.isBlack() && Vector3<float>.AbsDot(wi, inter.Shading.N) != 0)
                return f * Li(ray, scene, sampler, depth + 1) * Vector3<float>.AbsDot(wi, inter.Shading.N) / pdf;
            */
            return Spectrum.Zero;
        }
    }
}