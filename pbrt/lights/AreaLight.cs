using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.lights
{
    public abstract class AreaLight : Light
    {
        public AreaLight(Transform lightTransform, int sampleCount = 1)
            : base((int)LightFlags.Area, lightTransform, sampleCount)
        {
        }

        // Evaluatethe emitted radiance at a surface point in the given outgoing direction
        public abstract Spectrum L(Interaction inter, Vector3<float> w);
    }
}
