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

        // Sample a point on the surface of the shape.
        // Uses a probability density with respect to surface area on the shape.
        public abstract Interaction Sample(Point2<float> u);

        public virtual float Pdf(Interaction inter)
        {
            // Default implementation: samples uniformly over the shape's surface
            return 1f / Area();
        }

        // Sample a point on the surface of the shape.
        // Uses a probability density with respect to solid angle from the reference point.
        //
        // The reference point corresponds to the point to be lit. This information might
        // Be used by implementations that want to ensure that they sample portions of the shape
        // that are visible from that point.
        public abstract Interaction Sample(Interaction inter, Point2<float> u);

        public virtual float Pdf(Interaction inter, Vector3<float> wi)
        {
            var ray = inter.SpawnRay(wi);
            if (!Intersect(ray, out float tHit, out SurfaceInteraction intersection))
                return 0;

            return (inter.P - intersection.P).LengthSquared() /
                (Vector3<float>.AbsDot(-wi, intersection.N) * Area());
        }
    }
}
