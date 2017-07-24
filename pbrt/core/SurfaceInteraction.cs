using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public class SurfaceInteraction : Interaction
    {
        public Point2<float> Uv;
        public Vector3<float> DpDu, DpDv;
        public Normal3<float> DnDu, DnDv;

        public Shape Shape;
        public Primitive Primitive;
        public BSDF Bsdf;

        public struct SurfaceShading
        {
            public Normal3<float> N;
            public Vector3<float> DpDu, DpDv;
            public Normal3<float> DnDu, DnDv;
        }
        public SurfaceShading Shading;

        public SurfaceInteraction(Point3<float> p)
            : base(p, new Normal3<float>(), Vector3<float>.Zero, Vector3<float>.Zero, 0)
        {
        }

        public SurfaceInteraction(Point3<float> p, Vector3<float> pError, Point2<float> uv, Vector3<float> wo,
            Vector3<float> dpdu, Vector3<float> dpdv, Normal3<float> dndu, Normal3<float> dndv,
            float time, Shape shape)
            : base(p, new Normal3<float>(Vector3<float>.Cross(dpdu, dpdv).Normalized()), pError, wo, time)
        {
            Uv = uv;
            DpDu = dpdu;
            DpDv = dpdv;
            DnDu = dndu;
            DnDv = dndv;
            Shape = shape;

            // Initialize the shading data from the true geometry
            Shading.N = N;
            Shading.DpDu = DpDu;
            Shading.DpDv = DpDv;
            Shading.DnDu = DnDu;
            Shading.DnDv = DnDv;
        }

        public void SetShadingGeometry(Vector3<float> dpdu, Vector3<float> dpdv, Normal3<float> dndu, Normal3<float> dndv)
        {
            throw new NotImplementedException();
        }

        public void ComputeScatteringFunctions(RayDifferential ray, bool allowMultipleLobes = false/*, TransportMode mode*/)
        {
            // TODO for texture aliasing ComputeDifferentials(ray);
            Primitive.ComputeScatteringFunctions(this, allowMultipleLobes);
        }

        public Spectrum Le(Vector3<float> w)
        {
            return Spectrum.Zero;
            //var areaLight = Primitive.GetAreaLight();
            //return areaLight != null ? area.L() : Spectrum.Black();
        }
    }
}
