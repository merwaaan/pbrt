using pbrt.core;
using System;

namespace pbrt.integrators
{
    class DummyIntegrator : SamplerIntegrator
    {
        public DummyIntegrator(Sampler sampler, Window window = null)
            : base(sampler)
        {
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, Camera camera, Sampler sampler, int depth = 0)
        {
            var r = (float)Window.Random.NextDouble();
            return new Spectrum(r);
        }
    }
}
