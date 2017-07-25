using pbrt.core.geometry;
using System.Collections.Generic;
using System.Linq;

namespace pbrt.core
{
    // Represents a collection of BRDF and BTDF
    public class BSDF
    {
        public readonly float Eta;

        // Surface normal (from shading and from geometry)
        private Normal3<float> ns, ng;

        // Other basis vectors of the local coordinate frames
        private Vector3<float> ss, ts;

        private List<BxDF> bxdfs = new List<BxDF>();

        public BSDF(SurfaceInteraction inter, float eta = 1)
        {
            Eta = eta;

            ng = inter.N;
            ns = inter.Shading.N;

            ss = inter.Shading.DpDu.Normalized();
            ts = Vector3<float>.Cross(ns.ToVector3(), ss);
        }

        public void Add(BxDF b)
        {
            bxdfs.Add(b);
        }

        public int NumComponents(BxDF.BxDFType type = BxDF.BxDFType.All)
        {
            return bxdfs.Count(b => b.Matches(type));
        }

        public BxDF FirstMatch(BxDF.BxDFType type = BxDF.BxDFType.All)
        {
            return bxdfs.FirstOrDefault(b => b.Matches(type));
        }

        public Spectrum f(Vector3<float> woWorld, Vector3<float> wiWorld, BxDF.BxDFType type = BxDF.BxDFType.All)
        {
            // Transform the incident and outgoing directions to local BSDF space
            var wo = WorldToLocal(woWorld);
            var wi = WorldToLocal(wiWorld);

            var f = Spectrum.Zero;

            // Evaluate each BxDF
            var reflect = Vector3<float>.Dot(wiWorld, ng) * Vector3<float>.Dot(woWorld, ng) > 0;
            foreach (var bxdf in bxdfs)
                // Two possibilities:
                //   - wo and wi lie in the same hemisphere: only evaluate reflection
                //   - wo and wi lie in different hemispheres: only evaluate transmission
                if (bxdf.Matches(type) && (
                    reflect && (bxdf.Type & BxDF.BxDFType.Reflection) > 0 ||
                    !reflect && (bxdf.Type & BxDF.BxDFType.Transmission) > 0))
                    f += bxdf.f(wo, wi);

            return f;
        }

        public Spectrum Sample_f(Vector3<float> woWorld, out Vector3<float> wiWorld, Point2<float> sample, out float pdf, BxDF.BxDFType type, out BxDF.BxDFType sampledType)
        {
            // Check that there is at least one matching BxDF
            var matchingBxdf = FirstMatch(type);
            if (matchingBxdf == null)
            {
                pdf = 0;
                sampledType = 0;
                wiWorld = Vector3<float>.Zero;
                return Spectrum.Zero;
            }

            sampledType = matchingBxdf.Type;

            // Sample an incoming direction from the BxDF
            var wo = WorldToLocal(woWorld);
            var f = matchingBxdf.Sample_f(wo, out Vector3<float> wi, sample, out pdf, out sampledType);

            if (pdf == 0)
            {
                // TODO
            }

            wiWorld = LocalToWorld(wi);

            // TODO pdf weight

            // Compute the BDSF value for the sampled incoming direction
            // TODO for more complex BSDF
            /*
            var f = Spectrum.Zero;
            var reflect = Vector3<float>.Dot(wiWorld, ng) * Vector3<float>.Dot(woWorld, ng) > 0;
            foreach (var bxdf in bxdfs)
                if (bxdf.Matches(type) && (
                    reflect && (bxdf.Type & BxDF.BxDFType.Reflection) > 0 ||
                    !reflect && (bxdf.Type & BxDF.BxDFType.Transmission) > 0))
                    f += bxdf.f(wo, wi);
            */

            return f;
        }

        private Vector3<float> WorldToLocal(Vector3<float> v)
        {
            return new Vector3<float>(Vector3<float>.Dot(v, ss), Vector3<float>.Dot(v, ts), Vector3<float>.Dot(v, ns));
        }

        private Vector3<float> LocalToWorld(Vector3<float> v)
        {
            return new Vector3<float>(
                ss.X * v.X + ts.X * v.Y + ns.X * v.Z,
                ss.Y * v.X + ts.Y * v.Y + ns.Y * v.Z,
                ss.Z * v.X + ts.Z * v.Y + ns.Z * v.Z);
        }
    }
}
