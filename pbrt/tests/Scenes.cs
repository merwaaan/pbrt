using pbrt.cameras;
using pbrt.core;
using pbrt.core.geometry;
using pbrt.lights;
using pbrt.materials;
using pbrt.shapes;
using System;
using System.Collections.Generic;

namespace pbrt
{
    public struct SceneDescription
    {
        public Scene Scene;
        public Camera Camera;

        public SceneDescription(Aggregate aggr, IEnumerable<Light> lights, Camera camera)
        {
            Scene = new Scene(aggr, lights);
            Camera = camera;
        }
    }

    static class Scenes
    {
        public static SceneDescription Spheres()
        {
            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Sphere(Transform.Identity, 0.5f), new MatteMaterial(new Spectrum(1), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-0.5f, 0.5f, 0.5f), 0.25f), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.3f, 0.6f, -0.5f), 0.1f), new MatteMaterial(new Spectrum(0, 1, 0), 0))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(2, 5, -2), new Spectrum(30)),
                new PointLight(Transform.Translate(-3, -3, 1), new Spectrum(30))
            };

            return new SceneDescription(new PrimitiveList(primitives), lights, OrthographicCamera.Create(new Point3<float>(0, 0, -5)));
        }

        public static SceneDescription SpheresReflection(float roughness = 0.01f)
        {
            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Sphere(Transform.Identity, 0.5f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughness)),

                new GeometricPrimitive(new Sphere(Transform.Translate(1, 0, 0), 0.25f), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(-1, 0, 0), 0.25f), new MatteMaterial(new Spectrum(0, 1, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0, 0.75f, 0), 0.25f), new MatteMaterial(new Spectrum(0, 0, 10), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0, -1, 0), 0.25f), new MatteMaterial(new Spectrum(1, 1, 0), 0)),

                new GeometricPrimitive(new Sphere(Transform.Translate(-0.7f, 0.7f, 0), 0.25f), new MirrorMaterial(new Spectrum(1))),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.7f, 0.7f, 0), 0.25f), new MirrorMaterial(new Spectrum(1))),
                new GeometricPrimitive(new Sphere(Transform.Translate(-0.7f, -0.7f, 0), 0.25f), new MirrorMaterial(new Spectrum(1))),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.7f, -0.7f, 0), 0.25f), new MirrorMaterial(new Spectrum(1))),
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(1, 1, -5), new Spectrum(25)),
                new PointLight(Transform.Translate(-3, -3, 3), new Spectrum(10)),
            };

            return new SceneDescription(new PrimitiveList(primitives), lights, OrthographicCamera.Create(new Point3<float>(0, 0, -5)));
        }

        public static SceneDescription ManySpheres()
        {
            var rnd = new Random(1969);

            var primitives = new List<Primitive>();
            for (var i = 0; i < 250; ++i)
            {
                var spectrum = new Spectrum((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());

                Material material;
                if (rnd.NextDouble() > 0.7)
                    material = new MirrorMaterial(spectrum);
                else
                    //material = new MatteMaterial(spectrum, 0);
                    material = new PlasticMaterial(spectrum, new Spectrum(1), (float)rnd.NextDouble());

                var transform = Transform.Translate((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1);
                var radius = (float)rnd.NextDouble() * 0.25f;

                primitives.Add(new GeometricPrimitive(new Sphere(transform, radius), material));
            }

            var lights = new List<Light>();
            for (var i = 0; i < 15; ++i)
            {
                var transform = Transform.Translate((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * -3 - 5);
                var spectrum = new Spectrum((float)rnd.NextDouble() * 5 + 5);

                lights.Add(new PointLight(transform, spectrum));
            }

            return new SceneDescription(new PrimitiveList(primitives), lights, OrthographicCamera.Create(new Point3<float>(0, 0, -5)));
        }

        public static SceneDescription SpheresRoughness(float f)
        {
            var rnd = new Random(1969);

            float[] roughnesses = { 0.001f, 0.01f, 0.05f, 0.1f, 0.2f, 0.4f, 0.8f };

            var primitives = new List<Primitive>();
            for (var z = -1; z < 2; ++z)
            {
                for (var x = -3; x < 4; ++x)
                {
                    var material = new PlasticMaterial(new Spectrum(1), new Spectrum(1), roughnesses[x + 3]);

                    primitives.Add(new GeometricPrimitive(new Sphere(Transform.Translate(x, 0, z * 2), 0.25f), material));
                }
            }

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 10, -3), new Spectrum(100)),
                new PointLight(Transform.Translate(-5, -5, 0), new Spectrum(50))
            };

            //var camera = OrthographicCamera.Create(new Point3<float>(0, 3, -5), 8);
            var camera = PerspectiveCamera.Create(new Point3<float>(0, 3, -4), f);

            return new SceneDescription(new PrimitiveList(primitives), lights, camera);
        }
    }
}
