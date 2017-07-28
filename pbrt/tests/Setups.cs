using pbrt.cameras;
using pbrt.core;
using pbrt.core.geometry;
using pbrt.filters;
using pbrt.integrators;
using pbrt.samplers;

namespace pbrt
{
    static class Setups
    {
        // Dummy integrator that outputs noise
        public static Integrator Dummy()
        {
            return new DummyIntegrator(new PixelSampler(1, 0));
        }

        // Basic integrator that only outputs the scene's depth
        public static Integrator Depth()
        {
            return new DepthIntegrator(new PixelSampler(1, 0));
        }
        
        public static Integrator Whitted(int depth, int samplesPerPixel)
        {
            return new WhittedIntegrator(new PixelSampler(samplesPerPixel, 0), depth);
        }
    }
}
