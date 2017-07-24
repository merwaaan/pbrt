using pbrt.core;
using System;
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
            //Setups.Dummy().Render(null);
            //Setups.Depth().Render(Scenes.Spheres());
            //Setups.Whitted().Render(Scenes.Spheres());
            Do(Setups.Whitted(), Scenes.Spheres());
            Do(Setups.Whitted(), Scenes.Mirrors());

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
