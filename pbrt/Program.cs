using pbrt.core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pbrt
{
    class Program
    {
        public static Window Window;

        public const int Width = 800;
        public const int Height = 800;

        static void Main(string[] args)
        {
            //Do(Setups.Dummy(), null);
            //Do(Setups.Depth(), Scenes.Spheres());
            //Do(Setups.Whitted(), Scenes.Spheres());
            //Do(Setups.Whitted(), Scenes.ManySpheres());
            Do(Setups.Whitted(1), Scenes.Mirrors());
            Do(Setups.Whitted(2), Scenes.Mirrors());
            Do(Setups.Whitted(3), Scenes.Mirrors());

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

            //Thread.Sleep(1000);
        }
    }
}
