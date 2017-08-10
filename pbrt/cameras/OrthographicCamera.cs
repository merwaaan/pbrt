using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.cameras
{
    public class OrthographicCamera : ProjectiveCamera
    {
        // Amount of shifting when generating ray differentials
        Vector3<float> dxCamera, dyCamera;

        public OrthographicCamera(Transform cameraToWorld, Bounds2<float> screenWindow,
            float shutterOpen, float shutterClose, Film film)
            : base(cameraToWorld, Orthographic(0, 1), screenWindow, shutterOpen, shutterClose, film)
        {
            dxCamera = RasterToCamera * new Vector3<float>(1, 0, 0);
            dyCamera = RasterToCamera * new Vector3<float>(0, 1, 0);
        }

        private static Transform Orthographic(float zNear, float zFar)
        {
            // Orthographic viewing transformation:
            //   1 - Translate the scene along z to place the near plane at z = 0
            //   2 - Scale the scene along z to place the far plane at z = 1 
            return
                Transform.Scale(1.0f, 1.0f, 1.0f / (zFar - zNear)) *
                Transform.Translate(new Vector3<float>(0, 0, -zNear));
        }

        public override float GenerateRay(CameraSample sample, out Ray ray)
        {
            // Transform the film position from raster space to camera space
            var posFilm = new Point3<float>(sample.PosFilm.X, sample.PosFilm.Y, 0);
            var posCamera = RasterToCamera * posFilm;

            // Create a ray directed towards the camera's Z-axis
            ray = new Ray(posCamera, new Vector3<float>(0, 0, 1));

            // TODO DoF

            //ray.time = ...
            //ray.medium = ...

            ray = CameraToWorld * ray;

            return 1;
        }

        public override float GenerateRayDifferential(CameraSample sample, out RayDifferential rayDiff)
        {
            // Transform the film position from raster space to camera space
            var posFilm = new Point3<float>(sample.PosFilm.X, sample.PosFilm.Y, 0);
            var posCamera = RasterToCamera * posFilm;

            // Create a ray directed towards the camera's Z-axis
            rayDiff = new RayDifferential(posCamera, new Vector3<float>(0, 0, 1));
            rayDiff.RxO = rayDiff.O + dxCamera;
            rayDiff.RyO = rayDiff.O + dyCamera;
            rayDiff.RxD = rayDiff.RyD = rayDiff.D;

            // TODO time, medium
            rayDiff.HasDifferentials = true;

            // Transform the ray from camera space to world space
            rayDiff = CameraToWorld * rayDiff;

            return 1;
        }

        public static OrthographicCamera Create(Point3<float> position, float size = 2)
        {
            var half = size / 2;
            var ratio = (float)Program.Width / Program.Height * half;

            var film = new Film(new Point2<int>(Program.Width, Program.Height), Program.CropWindow, Program.Filter);

            return new OrthographicCamera(
                Transform.LookAt(
                    position,
                    new Point3<float>(0, 0, 0),
                    new Vector3<float>(0, 1, 0)).Inverse(),
                new Bounds2<float>(new Point2<float>(-ratio, -half), new Point2<float>(ratio, half)),
                0, 0,
                film);
        }
    }
}
