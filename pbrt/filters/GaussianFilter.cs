using pbrt.core;
using pbrt.core.geometry;
using System;

namespace pbrt.filters
{
    public class GaussianFilter : Filter
    {
        private float alpha;
        private float expX, expY;

        public GaussianFilter(Vector2<float> radius, float alpha)
            : base(radius)
        {
            this.alpha = alpha;
            expX = (float)Math.Exp(-alpha * radius.X * radius.X);
            expY = (float)Math.Exp(-alpha * radius.Y * radius.Y);
        }

        public override float Evaluate(Point2<float> point)
        {
            return Gaussian(point.X, expX) * Gaussian(point.Y, expY);
        }

        private float Gaussian(float d, float exp)
        {
            return (float)Math.Max(0.0f, Math.Exp(-alpha * d * d) - exp);
        }
    }
}
