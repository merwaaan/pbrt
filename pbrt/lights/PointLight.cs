using pbrt.core;
using pbrt.core.geometry;
using System;

namespace pbrt.lights
{
    public class PointLight : Light
    {
        private Point3<float> position;
        private Spectrum intensity;

        public PointLight(Transform lightToWorld, Spectrum i)
            : base((int)LightFlags.DeltaPosition, lightToWorld)
        {
            position = lightToWorld * Point3<float>.Zero;
            intensity = i;
        }

        public override Spectrum Power()
        {
            return intensity * (float)(4 * Math.PI);
        }

        public override Spectrum Sample_Li(SurfaceInteraction inter, Point2<float> u, out Vector3<float> wi, out float pdf, out VisibilityTester visTester)
        {
            var lightToPoint = position - inter.P;
            wi = lightToPoint.Normalized();
            pdf = 1.0f;
            visTester = new VisibilityTester(inter, new SurfaceInteraction(position));

            return intensity * (1.0f / lightToPoint.LengthSquared());
        }
    }
}
