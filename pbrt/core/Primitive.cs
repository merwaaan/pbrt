﻿using pbrt.core.geometry;
using pbrt.lights;

namespace pbrt.core
{
    public abstract class Primitive
    {
        //public BSDF Bsdf;
        //public BSSRDF BSSRDF;

        public abstract Bounds3<float> WorldBounds { get; }

        public virtual AreaLight GetAreaLight()
        {
            return null;
        }

        //public abstract Material GetMaterial();

        public abstract bool Intersect(Ray ray, out SurfaceInteraction inter);
        public abstract bool IntersectP(Ray ray);

        public abstract void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes); // TODo transport mode
    }
}
