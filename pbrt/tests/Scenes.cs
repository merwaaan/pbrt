using pbrt.core;
using pbrt.core.geometry;
using pbrt.lights;
using pbrt.materials;
using pbrt.shapes;
using System.Collections.Generic;

namespace pbrt
{
    static class Scenes
    {
        public static Scene Spheres()
        {
            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Sphere(Transform.Identity, 0.5f), new MatteMaterial(new Spectrum(40, 40, 40), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-0.5f, 0.5f, 0.5f), 0.25f), new MatteMaterial(new Spectrum(50, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.3f, 0.6f, -0.5f), 0.1f), new MatteMaterial(new Spectrum(0, 50, 0), 0))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(2, 5, -2), new Spectrum(30)),
                new PointLight(Transform.Translate(-3, -3, 1), new Spectrum(30))
            };

            return new Scene(new PrimitiveList(primitives), lights);
        }

        public static Scene Mirrors()
        {
            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Sphere(Transform.Identity, 0.1f), new MatteMaterial(new Spectrum(40, 40, 40), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-1,0,0), 0.25f), new MirrorMaterial(new Spectrum(40, 40, 40))),
                new GeometricPrimitive(new Sphere(Transform.Translate(+1,0,0), 0.25f), new MirrorMaterial(new Spectrum(40, 40, 40)))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 3, -2), new Spectrum(30))
            };

            return new Scene(new PrimitiveList(primitives), lights);
        }
    }
}
