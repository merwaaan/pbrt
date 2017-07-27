using pbrt.core.geometry;

namespace pbrt.core
{
    public abstract class Shape
    {
        protected Transform ObjectToWorld;
        protected Transform WorldToObject;

        public Shape(Transform objectToWorld, Transform worldToObject)
        {
            ObjectToWorld = objectToWorld;
            WorldToObject = worldToObject;
        }

        public abstract Bounds3<float> ObjectBounds();

        public virtual Bounds3<float> WorldBounds()
        {
            return ObjectToWorld * ObjectBounds();
        }

        public abstract bool Intersect(Ray ray, out float t, out SurfaceInteraction inter);

        public bool IntersectP(Ray ray)
        {
            return Intersect(ray, out float tHit, out SurfaceInteraction inter);
        }

        public abstract float Area();
    }
}
