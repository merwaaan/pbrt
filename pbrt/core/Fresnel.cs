using System;

namespace pbrt.core
{
    // A fresnel model gives the fraction of incoming light that is reflected
    public abstract class Fresnel
    {
        public abstract Spectrum Evaluate(float cosThetaI);

        // Fresnel reflectance at the boundary between dielectric medium I and medium T
        public static Spectrum FrDielectric(float cosThetaI, float etaI, float etaT)
        {
            cosThetaI = Math.Max(0, Math.Min(1, cosThetaI));

            // Swap indices of refraction depending if the incident direction
            // is going in or out of the medium
            var entering = cosThetaI > 0;
            if (!entering)
            {
                var tmp = etaI;
                etaI = etaT;
                etaT = tmp;
                cosThetaI = Math.Abs(cosThetaI);
            }

            // Compute the angle of transmittance using Snell's law
            var sinThetaI = Math.Sqrt(Math.Max(0, 1 - cosThetaI * cosThetaI));
            var sinThetaT = etaI / etaT * sinThetaI;

            // Special case: total reflection
            if (sinThetaT >= 1)
                return new Spectrum(1);

            var cosThetaT = Math.Sqrt(Math.Max(0, 1 - sinThetaT * sinThetaT));

            float rParallel = (float)((etaT * cosThetaI - etaI * cosThetaT) / (etaT * cosThetaI + etaI * cosThetaT));
            float rPerp = (float)((etaI * cosThetaI - etaT * cosThetaT) / (etaI * cosThetaI + etaT * cosThetaT));

            return new Spectrum((rParallel * rParallel + rPerp * rPerp) / 2);
        }

        // Fresnel reflectance at the boundary between conductor medium I and medium T
        public static Spectrum FrConductor(float cosThetaI, Spectrum etaI, Spectrum etaT, Spectrum k)
        {
            return new Spectrum(1); // TODO
        }
    }

    public class FresnelDielectric : Fresnel
    {
        // Indices of refraction
        public float EtaI, EtaT;

        public FresnelDielectric(float etaI, float etaT)
        {
            EtaI = etaI;
            EtaT = etaT;
        }

        public override Spectrum Evaluate(float cosThetaI)
        {
            return FrDielectric(Math.Abs(cosThetaI), EtaI, EtaT);
        }
    }

    public class FresnelConductor : Fresnel
    {
        // Indices of refraction
        public Spectrum EtaI, EtaT;

        // Absorbtion coefficient
        public Spectrum K;

        public override Spectrum Evaluate(float cosThetaI)
        {
            return FrConductor(Math.Abs(cosThetaI), EtaI, EtaT, K);
        }
    }

    public class FresnelNoOp : Fresnel
    {
        public override Spectrum Evaluate(float cosThetaI)
        {
            return new Spectrum(1);
        }
    }
}
