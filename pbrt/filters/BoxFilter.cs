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

        public BoxFilter(float radius)
            : base(new Vector2<float>(radius, radius))
        {
        }

        public override float Evaluate(Point2<float> point)
        {
            return 1.0f;
        }
    }
}
