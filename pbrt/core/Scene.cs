using pbrt.core.geometry;
using System.Collections.Generic;

namespace pbrt.core
{
    public class Scene
    {
        public Primitive Aggregate { get; private set; }
        public List<Light> Lights { get; private set; }

        public Bounds3<float> WorldBounds => Aggregate.WorldBounds;

        public Scene(Primitive aggregate, List<Light> lights)
        {
            Aggregate = aggregate;

            Lights = lights;
            foreach (var light in Lights)
                light.Preprocess();
        }

        public bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            return Aggregate.Intersect(ray, out inter);
        }

        public bool IntersectP(Ray ray)
        {
            return Aggregate.IntersectP(ray);
        }
    }
}
