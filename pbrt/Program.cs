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
            //Do(Setups.Whitted(), Scenes.Spheres());
            //Do(Setups.Whitted(), Scenes.ManySpheres());
            //Do(Setups.Whitted(2, 1), Scenes.SpheresReflection());
            /*Do(Setups.Whitted(2, 10000), Scenes.SpheresReflection(0.1f));
            Do(Setups.Whitted(2, 10000), Scenes.SpheresReflection(0.01f));
            Do(Setups.Whitted(2, 10000), Scenes.SpheresReflection(0.2f));*/
            Do(Setups.Whitted(2, 100, new BoxFilter(1)), Scenes.SpheresReflection(0.4f));
            //Do(Setups.Whitted(2, 10, new TriangleFilter(0.3f)), Scenes.SpheresReflection(0.4f));
            //Do(Setups.Whitted(3, 101), Scenes.SpheresReflection(0.01f));
            //Do(Setups.Whitted(3, 102), Scenes.SpheresReflection(0.1f));
            //Do(Setups.Whitted(2, 60), Scenes.SpheresReflection());

            Console.Read();
        }

        private static void Do(Integrator integrator, Scene scene)
        {
            Task.Factory.StartNew(() =>
            {
                var window = new Window(integrator, scene);

                if (Window == null)
                    Window = window;

                window.Run();
            });
        }
    }
}
