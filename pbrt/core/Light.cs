using pbrt.core.geometry;

namespace pbrt.core
{
    public abstract class Light
    {
        public enum LightFlags
        {
            DeltaPosition = 1,
            DeltaDirection = 2,
            Area = 4,
            Infinite = 8
        }

        public readonly int Flags;
        public readonly int SampleCount;

        protected Transform lightToWorld, worldToLight;

        public Light(int flags, Transform lightTransform, int sampleCount = 1)
        {
            Flags = flags;
            SampleCount = sampleCount;
            lightToWorld = lightTransform;
            worldToLight = lightToWorld.Inverse();
        }

        public abstract Spectrum Power();

        public abstract Spectrum Sample_Li(Interaction inter, Point2<float> u, out Vector3<float> wi, out float pdf, out VisibilityTester vis);
        public abstract float Pdf_Li(Interaction inter, Vector3<float> wi);

        public virtual void Preprocess()
        {
        }

        public bool IsDeltaLight()
        {
            return
                (Flags & (int)LightFlags.DeltaPosition) > 0 ||
                (Flags & (int)LightFlags.DeltaDirection) > 0;
        }

        public virtual Spectrum Le(RayDifferential ray)
        {
            return Spectrum.Zero;
        }
    }

    public class VisibilityTester
    {
        public readonly Interaction I0;
        public readonly Interaction I1;

        public VisibilityTester(Interaction i0, Interaction i1)
        {
            I0 = i0;
            I1 = i1;
        }

        public bool Unoccluded(Scene scene)
        {
            return !scene.IntersectP(I0.SpawnRayTo(I1));
        }

        // Compute the beam transmittance (accounts for attenuation from scattering media)
        public Spectrum Tr(Scene scene, Sampler sampler)
        {
            var ray = I0.SpawnRayTo(I1);
            var tr = new Spectrum(1);

            while (true)
            {
                var hit = scene.Intersect(ray, out SurfaceInteraction inter);

                // If an opaque material is hit, the ray is blocked
                if (hit /*&& inter.Primitive.GetMaterial() != null*/)
                    return Spectrum.Zero;

                // Accumulate transmittance
                //if (ray.Medium)
                //    tr *= ray.Medium.Tr(ray, sampler);

                // Stop when the ray hits the destination
                if (!hit)
                    break;

                // Or trace another ray to find other obstacles
                ray = inter.SpawnRayTo(I1);
            }

            return tr;
        }
    }
}
