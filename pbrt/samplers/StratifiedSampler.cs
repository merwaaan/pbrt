using pbrt.core;
using System;
/*
namespace pbrt.samplers
{
    class StratifiedSampler : PixelSampler
    {
        public int XSamples { get; }
        public int YSamples { get; }
        public bool Jitter { get; }

        public StratifiedSampler(int xSamples, int ySamples, bool jitter, int sampledDimensions)
            : base(xSamples * ySamples, sampledDimensions)
        {
            XSamples = xSamples;
            YSamples = ySamples;
            Jitter = jitter;
        }

        public override void StartPixel(Vector2 p)
        {
            for (var i = 0; i < samples1D.Count; ++i)
            {
                StratifiedSample1D(?);
                Shuffle();
            }

            for (var i = 0; i < samples2D.Count; ++i)
            {
                StratifiedSample2D(?);
                Shuffle();
            }

            base.StartPixel(p);
        }

        private void StratifiedSample1D(float sample)
        {
            var nSamples = XSamples * YSamples;
            var invNSamples = 1.0f / nSamples;

            for (var i = 0; i < nSamples; ++i)
            {
                float delta = Jitter ? (float)random.NextDouble() : 0.5f;
                XXX = Math.Min((i + delta) * invNSamples, OneMinusEpsilon);
            }
        }

        // TODO 2D
    }
}
*/