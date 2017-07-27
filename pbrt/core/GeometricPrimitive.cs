using pbrt.core.geometry;

namespace pbrt.core
{
    public class GeometricPrimitive : Primitive
    {
        public readonly Shape Shape;
        public readonly Material Material;

        public override Bounds3<float> WorldBounds => Shape.WorldBounds();

        //private AreaLight areaLight;
        //private MediumInterface medium;

        public GeometricPrimitive(Shape shape, Material material)
        {
            Shape = shape;
            Material = material;
        }

        public override bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            if (!Shape.Intersect(ray, out float tHit, out inter))
                return false;

            ray.Tmax = tHit;
            inter.Primitive = this;
            return true;
        }

        public override bool IntersectP(Ray ray)
        {
            return Shape.IntersectP(ray);
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            if (Material != null)
                Material.ComputeScatteringFunctions(inter, allowMultipleLobes);
        }

        /*public override AreaLight GetAreaLight()
        {
            return areaLight;
        }*/
    }
}
