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

        public static Scene ManySpheres()
        {
            var primitives = new List<Primitive>();

            for (var z = -2; z < 3; ++z)
                for (var x = -2; x < 3; ++x)
                    primitives.Add(new GeometricPrimitive(new Sphere(Transform.Translate(x * 0.25f, 0, z * 0.25f), 0.1f), new MatteMaterial(new Spectrum(40, 40, 40), 0)));

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 3, -2), new Spectrum(30))
            };

            return new Scene(new PrimitiveList(primitives), lights);
        }

        public static Scene SpheresReflection()
        {
            var roughness = 0.15f; // tmp roughness, should be remapped

            var primitives = new List<Primitive>()
            {
                //new GeometricPrimitive(new Sphere(Transform.Identity, 0.5f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness)),
                new GeometricPrimitive(new Sphere(Transform.Identity, 0.5f), new MirrorMaterial(new Spectrum(1))),

                new GeometricPrimitive(new Sphere(Transform.Translate(1, 0, 0), 0.25f), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-1, 0, 0), 0.25f), new MatteMaterial(new Spectrum(0, 1, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0, 1, 0), 0.25f), new MatteMaterial(new Spectrum(0, 0, 1), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0, -1, 0), 0.25f), new MatteMaterial(new Spectrum(1, 1, 0), 0)),

                new GeometricPrimitive(new Sphere(Transform.Translate(-0.7f, 0.7f, 0), 0.15f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.7f, 0.7f, 0), 0.15f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-0.7f, -0.7f, 0), 0.15f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.7f, -0.7f, 0), 0.15f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(1, 1, -5), new Spectrum(25)),
                new PointLight(Transform.Translate(-3, -3, 3), new Spectrum(10)),
            };

            return new Scene(new PrimitiveList(primitives), lights);
        }
    }
}
