using pbrt.core.geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static pbrt.core.BxDF;

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

        public Spectrum Sample_f(Vector3<float> woWorld, out Vector3<float> wiWorld, Point2<float> u, out float pdf, BxDF.BxDFType type, out BxDF.BxDFType sampledType)
        {
            // Check that there are matching BxDFs
            var matchingBxdfs = bxdfs.Count(b => b.Matches(type));
            if (matchingBxdfs == 0)
            {
                pdf = 0;
                sampledType = 0;
                wiWorld = Vector3<float>.Zero;
                return Spectrum.Zero;
            }

            // Randomly pick a matching BxDF with the first dimension of the sample
            var n = (int)Math.Min(Math.Floor(u.X * matchingBxdfs), matchingBxdfs - 1);
            var pickedBxdf = bxdfs.Where(b => b.Matches(type)).Skip(n).First();
            sampledType = pickedBxdf.Type;

            // Remap the sample to [0,1]
            u.X = u.X * matchingBxdfs - n;

            // Sample an incoming direction from the BxDF
            var wo = WorldToLocal(woWorld);
            var f = pickedBxdf.Sample_f(wo, out Vector3<float> wi, u, out pdf, out sampledType);
            wiWorld = LocalToWorld(wi);

            if (pdf == 0)
                return Spectrum.Zero;

            // Compute overall PDF with matching BxDFs
            if ((pickedBxdf.Type & BxDFType.Specular) == 0 && matchingBxdfs > 1)
                foreach (var b in bxdfs.Where(b => b != pickedBxdf && b.Matches(type)))
                    pdf += b.Pdf(wo, wi);

            if (matchingBxdfs > 1)
                pdf /= matchingBxdfs;

            // Compute the BDSF value for the sampled incoming direction
            if ((pickedBxdf.Type & BxDFType.Specular) == 0 && matchingBxdfs > 1)
            {
                var reflect = Vector3<float>.Dot(wiWorld, ng) * Vector3<float>.Dot(woWorld, ng) > 0;
                foreach (var bxdf in bxdfs)
                    if (bxdf.Matches(type) && (
                        reflect && (bxdf.Type & BxDFType.Reflection) > 0 ||
                        !reflect && (bxdf.Type & BxDFType.Transmission) > 0))
                        f += bxdf.f(wo, wi);
            }

            return f;
        }

        public float Pdf(Vector3<float> woWorld, Vector3<float> wiWorld, BxDFType type)
        {
            if (!bxdfs.Any())
                return 0;

            var wo = WorldToLocal(woWorld);
            var wi = WorldToLocal(wiWorld);

            if (wo.Z == 0)
                return 0;

            var matchingBxdfs = bxdfs.Where(b => b.Matches(type));
            var pdf = 0f;
            foreach (var b in bxdfs)
                pdf += b.Pdf(wo, wi);

            var count = matchingBxdfs.Count();
            return count > 0 ? pdf / count : 0;
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
