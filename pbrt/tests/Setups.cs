using pbrt.cameras;
using pbrt.core;
using pbrt.core.geometry;
using pbrt.integrators;
using pbrt.samplers;

namespace pbrt
{
    static class Setups
    {
        // Dummy integrator that outputs noise
        public static Integrator Dummy()
        {
            var film = new Film(new Point2<int>(800, 800), new Bounds2<float>(Point2<float>.Zero, Point2<float>.One));
            var ratio = (float)Program.Width / Program.Height;
            var camera = new OrthographicCamera(
                Transform.LookAt(new Point3<float>(0, 0, -5), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse(),
                new Bounds2<float>(new Point2<float>(-ratio, -1), new Point2<float>(ratio, 1)),
                0, 0, film);

            return new DummyIntegrator(new PixelSampler(1, 0), camera);
        }

        // Basic integrator that only outputs the scene's depth
        public static Integrator Depth()
        {
            var film = new Film(new Point2<int>(800, 800), new Bounds2<float>(Point2<float>.Zero, Point2<float>.One));
            var ratio = (float)Program.Window.Width / Program.Window.Height;
            var camera = new OrthographicCamera(
                Transform.LookAt(new Point3<float>(0, 0, -5), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse(),
                new Bounds2<float>(new Point2<float>(-ratio, -1), new Point2<float>(ratio, 1)),
                0, 0, film);

            return new DepthIntegrator(new PixelSampler(1, 0), camera);
        }
        
        public static Integrator Whitted(int depth)
        {
            var film = new Film(new Point2<int>(800, 800), new Bounds2<float>(Point2<float>.Zero, Point2<float>.One));
            var ratio = (float)Program.Width / Program.Height;
            var camera = new OrthographicCamera(
                Transform.LookAt(new Point3<float>(0, 0, -5), new Point3<float>(0, 0, 0), new Vector3<float>(0, 1, 0)).Inverse(),
                new Bounds2<float>(new Point2<float>(-ratio, -1), new Point2<float>(ratio, 1)),
                0, 0, film);

            return new WhittedIntegrator(camera, new PixelSampler(1, 0), depth);
        }
    }
}
