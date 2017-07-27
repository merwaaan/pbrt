using pbrt.core.geometry;
using System;

namespace pbrt.core
{// TODO world bounds here?
    public abstract class Aggregate : Primitive
    {
        public override bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectP(Ray ray)
        {
            throw new NotImplementedException();
        }
    }
}
