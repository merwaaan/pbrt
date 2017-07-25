using pbrt.core.geometry;

namespace pbrt
{
    public class Interaction
    {
        public Point3<float> P;
        public Vector3<float> PError;
        public Normal3<float> N;
        public Vector3<float> Wo;
        public float Time;

        public Interaction(Point3<float> p, Normal3<float> n, Vector3<float> pError, Vector3<float> wo, float time/*, MediumInterface medium*/)
        {
            P = p;
            N = n;
            PError = pError;
            Wo = wo;
            Time = time;
        }

        public bool IsSurfaceInteraction()
        {
            return N.X != 0 && N.Y != 0 && N.Z != 0;
        }

        // Create a ray originating from this interaction point and going to the given one.
        public Ray SpawnRay(Vector3<float> dir)
        {
            var origin = P + dir.Normalized() * 0.01f; // Add offset to be on the right side of the surface

            return new Ray(origin, dir.Normalized());
        }

        // Create a ray originating from this interaction point and going to the given one.
        public Ray SpawnRayTo(Interaction inter)
        {
            var dir = (inter.P - P).Normalized();
            var origin = P + dir * 0.01f; // Add offset to be on the right side of the surface
            var target = inter.P - dir * 0.01f;

            return new Ray(origin, (target - origin).Normalized());
        }
    }
}
