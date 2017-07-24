using pbrt.core;
using pbrt.core.geometry;

namespace pbrt
{
    public abstract class Camera
    {
        public Film Film { get; private set; }

        public Transform CameraToWorld;
        public float ShutterOpen, ShutterClose;
        // medium

        public Point3<float> Position => new Point3<float>(CameraToWorld.M.M14, CameraToWorld.M.M24, CameraToWorld.M.M34);

        public struct CameraSample
        {
            public Point2<float> PosFilm;
            public Point2<float> PosLens;
            public float Time;
        }

        public Camera(Transform cameraToWorld, float shutterOpen, float shutterClose, Film film)
        {
            CameraToWorld = cameraToWorld;
            ShutterOpen = shutterOpen;
            ShutterClose = shutterClose;
            Film = film;
        }

        public abstract float GenerateRay(CameraSample sample, out Ray ray);

        public virtual float GenerateRayDifferential(CameraSample sample, out RayDifferential rayDiff)
        {
            rayDiff = null;

            Ray ray;
            var weight = GenerateRay(sample, out ray);

            // Compute the ray shifted on the X-axis
            var shiftedX = sample;
            shiftedX.PosFilm.X++;
            Ray rayX;
            var weightX = GenerateRay(shiftedX, out rayX);
            if (weightX == 0)
                return 0;
            rayDiff.RxO = rayX.O;
            rayDiff.RxD = rayX.D;

            // Compute the ray shifted on the Y-axis
            var shiftedY = sample;
            shiftedY.PosFilm.X--;
            shiftedY.PosFilm.Y++;
            Ray rayY;
            var weightY = GenerateRay(shiftedY, out rayY);
            if (weightY == 0)
                return 0;
            rayDiff.RyO = rayY.O;
            rayDiff.RyD = rayY.D;

            rayDiff.HasDifferentials = true;
            return weight;
        }
    }
}
