using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.filters
{
    public class BoxFilter : Filter
    {
        public BoxFilter(Vector2<float> radius)
            : base(radius)
        {
        }

        public override float Evaluate(Point2<float> point)
        {
            return 1.0f;
        }
    }
}
