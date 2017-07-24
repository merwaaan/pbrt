using pbrt.core;
using System;

namespace pbrt.integrators
{
    class DummyIntegrator : SamplerIntegrator
    {
        public DummyIntegrator(Sampler sampler, Camera camera, Window window = null)
            : base(sampler, camera)
        {
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, Sampler sampler, int depth = 0)
        {
            var r = (float)new Random(DateTime.Now.Millisecond).NextDouble();
            return new Spectrum(r);
        }
    }
}
