using pbrt.core;

namespace pbrt.integrators
{
    class DepthIntegrator : SamplerIntegrator
    {
        public DepthIntegrator(Sampler sampler, Camera camera)
            : base(sampler, camera)
        {
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, Sampler sampler, int depth = 0)
        {
            // If the ray doesn't intersect the scene, return black
            if (!scene.Intersect(ray, out SurfaceInteraction inter))
                return Spectrum.Zero;

            // Otherwise return a greyscale level that corresponds to
            // the object's distance from the camera
            float c = (inter.P - Camera.Position).Length() / 3.0f;
            return new Spectrum(c);
        }
    }
}
