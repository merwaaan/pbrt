using OpenTK;
using pbrt.core;
using pbrt.core.geometry;
using System;

namespace pbrt.cameras
{
    public class PerspectiveCamera : ProjectiveCamera
    {
        // Amount of shifting when generating ray differentials
        Vector3<float> dxCamera, dyCamera;

        public PerspectiveCamera(Transform cameraToWorld, Bounds2<float> screenWindow,
            float shutterOpen, float shutterClose, float fov, Film film)
            : base(cameraToWorld, Perspective(fov, 0.001f, 1000f), screenWindow, shutterOpen, shutterClose, film)
        {
            dxCamera = RasterToCamera * new Vector3<float>(1, 0, 0) - RasterToCamera * Vector3<float>.Zero;
            dyCamera = RasterToCamera * new Vector3<float>(0, 1, 0) - RasterToCamera * Vector3<float>.Zero;
        }

        private static Transform Perspective(float fov, float zNear, float zFar)
        {
            // Perspective viewing transformation:
            //   1 - Translate the scene along z to place the near plane at z = 0 and the far plane at z = 1
            //   2 - Scale the scene depending on the Field of View and the Z-axis 

            var perspective = new Matrix4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, zFar / (zFar - zNear), -(zFar * zNear) / (zFar - zNear),
                0, 0, 1, 0);

            var invTanAng = 1.0f / (float)Math.Tan(MathUtils.Radians(fov) / 2);

            return
                Transform.Scale(invTanAng, invTanAng, 1) *
                new Transform(perspective);
        }

        public override float GenerateRay(CameraSample sample, out Ray ray)
        {
            // Transform the film position from raster space to camera space
            var posFilm = new Point3<float>(sample.PosFilm.X, sample.PosFilm.Y, 0);
            var posCamera = RasterToCamera * posFilm;

            // Create a ray coming from the center of the view
            ray = new RayDifferential(Point3<float>.Zero, new Vector3<float>(posCamera).Normalized());

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

            // Create a ray coming from the center of the view
            rayDiff = new RayDifferential(Point3<float>.Zero, new Vector3<float>(posCamera).Normalized());
            rayDiff.RxO = rayDiff.O + dxCamera;
            rayDiff.RyO = rayDiff.O + dyCamera;
            rayDiff.RxD = rayDiff.RyD = rayDiff.D;

            // TODO time, medium
            rayDiff.HasDifferentials = true;

            // Transform the ray from camera space to world space
            rayDiff = CameraToWorld * rayDiff;

            return 1;
        }

        public static PerspectiveCamera Create(Point3<float> position, float fov = 100)
        {
            var film = new Film(new Point2<int>(Program.Width, Program.Height), Program.CropWindow, Program.Filter);

            return new PerspectiveCamera(
                Transform.LookAt(
                    position,
                    new Point3<float>(0, 0, 0),
                    new Vector3<float>(0, 1, 0)).Inverse(),
                new Bounds2<float>(new Point2<float>(-1, -1), new Point2<float>(1, 1)),
                0, 0, fov,
                film);
        }
    }
}
