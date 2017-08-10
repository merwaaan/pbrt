using pbrt.accelerators;
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
                new GeometricPrimitive(new Sphere(Transform.Translate(-0.75f, 0.5f, 0.5f), 0.25f), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.3f, 0.25f, -0.5f), 0.1f), new MatteMaterial(new Spectrum(0, 1, 0), 0))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(2, 5, -2), new Spectrum(30)),
                new PointLight(Transform.Translate(0, -3, -3), new Spectrum(30))
            };

            return new SceneDescription(new PrimitiveList(primitives), lights, PerspectiveCamera.Create(new Point3<float>(0, 0, -1)));
        }

        public static SceneDescription Disk()
        {
            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Disk(Transform.Identity, 0.5f), new MatteMaterial(new Spectrum(1), 0))
            };

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 0, -5), new Spectrum(30))
            };

            return new SceneDescription(new PrimitiveList(primitives), lights, PerspectiveCamera.Create(new Point3<float>(0, 0, -1)));
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
            var primitives = new List<Primitive>();
            for (var i = 0; i < 200; ++i)
            {
                var spectrum = new Spectrum((float)Window.Random.NextDouble(), (float)Window.Random.NextDouble(), (float)Window.Random.NextDouble());

                Material material;
                if (Window.Random.NextDouble() > 0.7)
                    material = new MirrorMaterial(spectrum);
                else
                    //material = new MatteMaterial(spectrum, 0);
                    material = new PlasticMaterial(spectrum, new Spectrum(1), (float)Window.Random.NextDouble());

                var transform = Transform.Translate((float)Window.Random.NextDouble() * 2 - 1, (float)Window.Random.NextDouble() * 2 - 1, (float)Window.Random.NextDouble() * 2 - 1);
                var radius = 0.1f + (float)Window.Random.NextDouble() * 0.1f;

                primitives.Add(new GeometricPrimitive(new Sphere(transform, radius), material));
            }

            var lights = new List<Light>();
            for (var i = 0; i < 5; ++i)
            {
                var transform = Transform.Translate((float)Window.Random.NextDouble() * 2 - 1, (float)Window.Random.NextDouble() * 2 - 1, (float)Window.Random.NextDouble() * -3 - 5);
                var spectrum = new Spectrum((float)Window.Random.NextDouble() * 25 + 15);

                lights.Add(new PointLight(transform, spectrum));
            }

            return new SceneDescription(new PrimitiveList(primitives), lights, OrthographicCamera.Create(new Point3<float>(0, 0, -5)));
        }

        public static SceneDescription SpheresRoughness()
        {

            float[] roughnesses = { 0.001f, 0.01f, 0.05f, 0.1f, 0.2f, 0.4f, 0.8f };
            Spectrum[] colors = { new Spectrum(1, 0, 0), new Spectrum(0, 1, 0), new Spectrum(0, 0, 1) };

            var primitives = new List<Primitive>();
            for (var z = -1; z < 2; ++z)
            {
                for (var x = -3; x < 4; ++x)
                {
                    var material = new PlasticMaterial(colors[z + 1], new Spectrum(1), roughnesses[x + 3]);

                    primitives.Add(new GeometricPrimitive(new Sphere(Transform.Translate(x, 0, z), 0.25f), material));
                }
            }

            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 10, -3), new Spectrum(100)),
                new PointLight(Transform.Translate(-5, -5, 0), new Spectrum(50))
            };

            //var camera = OrthographicCamera.Create(new Point3<float>(0, 3, -5), 8);
            var camera = PerspectiveCamera.Create(new Point3<float>(0, 3, -4), 100);

            return new SceneDescription(new PrimitiveList(primitives), lights, camera);
        }

        public static SceneDescription IndirectLight()
        {
            // Single light source at the top
            var lights = new List<Light>()
            {
                new PointLight(Transform.Translate(0, 5, 0), new Spectrum(100))
            };

            var primitives = new List<Primitive>()
            {
                // Small object that should only receive indirect light
                // bouncing from the two side objects
                new GeometricPrimitive(new Sphere(Transform.Translate(0, -1.5f, 0), 0.5f), new PlasticMaterial(new Spectrum(10), new Spectrum(10), 0.01f)),

                // Big object between the light source and the small object
                new GeometricPrimitive(new Sphere(Transform.Identity, 1.0f), new PlasticMaterial(new Spectrum(1), new Spectrum(1), 0.1f)),

                // Side objects to reflect the light
                new GeometricPrimitive(new Sphere(Transform.Translate(-10, 0, 0), 6), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(10, 0, 0), 6), new MatteMaterial(new Spectrum(0,1,0), 0)),
            };

            var camera = PerspectiveCamera.Create(new Point3<float>(0, 0, -5), 100);

            return new SceneDescription(new PrimitiveList(primitives), lights, camera);
        }

        public static SceneDescription AreaLight(bool secondLight)
        {
            var lightTransform = Transform.LookAt(new Point3<float>(0.5f, 0.75f, -0.5f), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse();
            var lightShape = new Disk(lightTransform, 0.75f);
            var light = new DiffuseAreaLight(lightTransform, new Spectrum(5), lightShape, 1, true);

            var lightTransform2 = Transform.LookAt(new Point3<float>(-1, -1.5f, -1f), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse();
            var lightShape2 = new Disk(lightTransform2, 3);
            var light2 = new DiffuseAreaLight(lightTransform2, new Spectrum(1, 0.07f, 0.57f), lightShape2, 1, true);

            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Sphere(Transform.Translate(0, 0, 0), 0.5f), new MatteMaterial(new Spectrum(1), 0)),
                //new GeometricPrimitive(new Disk(Transform.Translate(0, -1f, 0), 2), new MatteMaterial(new Spectrum(1), 0)),

                new GeometricPrimitive(lightShape, new MatteMaterial(new Spectrum(1), 0), light)
            };

            if (secondLight)
                primitives.Add(new GeometricPrimitive(lightShape2, new MatteMaterial(new Spectrum(1), 0), light2));

            var lights = new List<Light>()
            {
                light
            };

            if (secondLight)
                lights.Add(light2);

            return new SceneDescription(new PrimitiveList(primitives), lights, OrthographicCamera.Create(new Point3<float>(0, 0, -5)));
        }

        public static SceneDescription LargeAreaLight()
        {
            var lightTransform = Transform.LookAt(new Point3<float>(0.5f, 1.5f, 0), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse();
            var lightShape = new Disk(lightTransform, 4);
            var light = new DiffuseAreaLight(lightTransform, new Spectrum(5), lightShape, 1, true);

            var primitives = new List<Primitive>()
            {
                new GeometricPrimitive(new Disk(Transform.Translate(0, -1, 0) * Transform.RotateX(90), 1.5f), new MatteMaterial(new Spectrum(1), 0)),

                new GeometricPrimitive(new Sphere(Transform.Translate(-0.75f, -0.5f, 0), 0.25f), new MatteMaterial(new Spectrum(1, 0, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0, -0.25f, 0), 0.25f), new MatteMaterial(new Spectrum(0, 1, 0), 0)),
                new GeometricPrimitive(new Sphere(Transform.Translate(0.75f, 0, 0), 0.25f), new MatteMaterial(new Spectrum(1, 1, 0), 0)),

                new GeometricPrimitive(lightShape, new MatteMaterial(new Spectrum(1), 0), light)
            };

            var lights = new List<Light>()
            {
                light
            };
            
            return new SceneDescription(new PrimitiveList(primitives), lights, PerspectiveCamera.Create(new Point3<float>(0, 1, -1), 100));
        }
    }
}
