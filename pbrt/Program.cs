using pbrt.core;
using pbrt.core.geometry;
using pbrt.filters;
using System;
using System.Threading.Tasks;

namespace pbrt
{
    class Program
    {
        public static Window Window;

        public const int Width = 800;
        public const int Height = 800;
        public const int ThreadCount = 20;
        public const int TileSize = 64;

        public static Filter Filter = new BoxFilter(new Vector2<float>(1.0f, 1.0f));

        public static Bounds2<float> CropWindow = new Bounds2<float>(
            Point2<float>.Zero,
            Point2<float>.One
            //new Point2<float>(0.4f, 0.25f),
            //new Point2<float>(0.6f, 0.4f)
            );

        static void Main(string[] args)
        {
            //Do(Setups.Dummy(), null);
            //Do(Setups.Depth(), Scenes.Spheres());
            //Do(Setups.Whitted(1, 10), Scenes.Spheres());
            //Do(Setups.Whitted(2, 100), Scenes.SpheresReflection(0.4f));
            //Do(Setups.Whitted(2, 100), Scenes.ManySpheres());
            Do(Setups.Whitted(1, 20), Scenes.SpheresRoughness(100));

            Console.Read();
        }

        private static void Do(Integrator integrator, SceneDescription sceneDesc)
        {
            Task.Factory.StartNew(() =>
            {
                var window = new Window(integrator, sceneDesc);

                if (Window == null)
                    Window = window;

                window.Run();
            });
        }
    }
}
