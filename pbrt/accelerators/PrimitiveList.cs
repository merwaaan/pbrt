using pbrt.core.geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace pbrt.core
{
    // Basic list of primitives, without acceleration structure
    public class PrimitiveList : Aggregate
    {
        public List<Primitive> Primitives { get; private set; }

        public override Bounds3<float> WorldBounds => worldBounds;

        private Bounds3<float> worldBounds;

        public PrimitiveList(IEnumerable<Primitive> primitives)
        {
            Primitives = new List<Primitive>(primitives);

            // Cache the world space bounds
            if (Primitives.Any())
            {
                worldBounds = Primitives.First().WorldBounds;
                foreach (var p in Primitives.Skip(1))
                    worldBounds = worldBounds.Union(p.WorldBounds);
            }
        }

        public override bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            var hit = false;
            float closestT = float.PositiveInfinity;
            inter = null;

            foreach (var p in Primitives)
                if (/*p.WorldBounds.IntersectP(ray, out float t0, out float t1) && */p.Intersect(ray, out SurfaceInteraction inter2) && ray.Tmax < closestT)
                {
                    hit = true;
                    closestT = ray.Tmax;
                    inter = inter2;
                }

            return hit;
        }

        public override bool IntersectP(Ray ray)
        {
            foreach (var p in Primitives)
                if (p.IntersectP(ray))
                    return true;

            return false;
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            throw new NotImplementedException();
        }
    }
}
