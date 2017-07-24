using pbrt.core;
using pbrt.core.geometry;

namespace pbrt
{
    public class Ray
    {
        public readonly Point3<float> O;
        public readonly Vector3<float> D;
        public readonly float Time;
        //public readonly Medium Medium;
        public float Tmax;

        public Ray()
        {
            Tmax = float.PositiveInfinity;
        }

        public Ray(Ray r)
        {
            O = r.O;
            D = r.D;
            Time = r.Time;
            Tmax = r.Tmax;
        }

        public Ray(Point3<float> origin, Vector3<float> direction, float tMax = float.PositiveInfinity, float time = 0.0f /*, Medium medium*/)
        {
            O = origin;
            D = direction;
            Tmax = tMax;
            Time = time;
            //Medium = medium;
        }

        // Returns the point at distance t along the ray
        public Point3<float> At(float t)
        {
            return O + D * t;
        }
    }

    public class RayDifferential : Ray
    {
        public Point3<float> RxO, RyO;
        public Vector3<float> RxD, RyD;
        public bool HasDifferentials;

        public RayDifferential(Point3<float> origin, Vector3<float> direction, float tMax = float.PositiveInfinity, float time = 0.0f)
            : base(origin, direction, tMax, time)
        {
            // initially set to false because the neighboring rays are not known
            HasDifferentials = false;
        }

        public void ScaleDifferentials(float spacing)
        {
            RxO = O + (RxO - O) * spacing;
            RxD = D + (RxD - D) * spacing;
            RyO = O + (RyO - O) * spacing;
            RyD = D + (RyD - D) * spacing;
        }
    }
}
