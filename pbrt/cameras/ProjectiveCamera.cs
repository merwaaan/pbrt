using pbrt.core;
using pbrt.core.geometry;

namespace pbrt.cameras
{
    public abstract class ProjectiveCamera : Camera
    {
        // Projection matrix
        protected Transform CameraToScreen;       

        // Transformations between:
        //   - screen space (Z-values in [0, 1])
        //   - raster space (X/Y-values in [res.x, res.y], Z-values in [0, 1])
        protected Transform ScreenToRaster, RasterToScreen;

        protected Transform RasterToCamera;

        public ProjectiveCamera(Transform cameraToWorld, Transform cameraToScreen, Bounds2<float> screenWindow,
            float shutterOpen, float shutterClose, Film film)
            : base(cameraToWorld, shutterOpen, shutterClose, film)
        {
            CameraToScreen = cameraToScreen;

            // Compute projective camera screen transformation (read from bottom to top)
            // (Y-axis coordinates are inverted because they move down from top to bottom in raster space)
            ScreenToRaster =
                // Scale the (x, y) coordinates to the image resolution
                Transform.Scale(film.Resolution.X, film.Resolution.Y, 1.0f) *
                // Scale (x, y) coordinates in [0, 1]
                Transform.Scale(1.0f / (screenWindow.Max.X - screenWindow.Min.X), 1.0f / (screenWindow.Min.Y - screenWindow.Max.Y), 1) *
                // Translate the upper-left corner of the screen to the origin
                Transform.Translate(-screenWindow.Min.X, -screenWindow.Max.Y, 0.0f);

            RasterToScreen = ScreenToRaster.Inverse();
            RasterToCamera = cameraToScreen.Inverse() * RasterToScreen;
        }
    }
}
