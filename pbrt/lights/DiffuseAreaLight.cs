using System;
using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.lights
{
    public class DiffuseAreaLight : AreaLight
    {
        public readonly bool TwoSided;

        protected Spectrum Lemit;
        protected Shape Shape;
        protected float Area;

        public DiffuseAreaLight(Transform lightTransform, Spectrum l, Shape shape, int sampleCount, bool twoSided = false)
            : base(lightTransform, sampleCount)
        {
            TwoSided = twoSided;
            Lemit = l;
            Shape = shape;
            Area = shape.Area();
        }

        public override Spectrum L(Interaction inter, Vector3<float> w)
        {
            return (TwoSided || inter.N.ToVector3().Dot(w) > 0) ? Lemit : Spectrum.Zero;
        }

        public override Spectrum Power()
        {
            return Lemit * Area * MathUtils.Pi;
        }

        public override Spectrum Sample_Li(Interaction inter, Point2<float> u, out Vector3<float> wi, out float pdf, out VisibilityTester vis)
        {
            Interaction interShape = Shape.Sample(u);
            wi = (interShape.P - inter.P).Normalized();
            pdf = Shape.Pdf(inter, wi);
            vis = new VisibilityTester(inter, interShape);
            return L(interShape, -wi);
        }

        public override float Pdf_Li(Interaction inter, Vector3<float> wi)
        {
            return Shape.Pdf(inter, wi);
        }
    }
}
