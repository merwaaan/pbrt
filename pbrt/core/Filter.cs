using pbrt.core.geometry;

namespace pbrt.core
{
    public abstract class Filter
    {
        public Vector2<float> Radius, InvRadius;

        protected Filter(Vector2<float> radius)
        {
            Radius = radius;
            InvRadius = new Vector2<float>(1.0f / radius.X, 1.0f / radius.Y);
        }

        public abstract float Evaluate(Point2<float> point);
    }
}
