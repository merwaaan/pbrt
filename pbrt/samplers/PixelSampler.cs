using pbrt.core;
using System.Collections.Generic;
using System;
using pbrt.core.geometry;

namespace pbrt.samplers
{
    public class PixelSampler : Sampler
    {
        protected List<float[]> Samples1D = new List<float[]>();
        private int current1DDimension;

        protected List<Point2<float>[]> Samples2D = new List<Point2<float>[]>();
        private int current2DDimension;

        protected Random Random;

        public PixelSampler(int samplesPerPixel, int sampleDimensions)
            : base(samplesPerPixel)
        {
            for (var i = 0; i < sampleDimensions; ++i)
            {
                Samples1D.Add(new float[samplesPerPixel]);
                Samples2D.Add(new Point2<float>[samplesPerPixel]);
            }
        }

        public override Sampler Clone(int seed)
        {
            var clone = new PixelSampler(SamplesPerPixel, Samples1D.Count);
            clone.Random = new Random(seed);
            return clone;
        }

        public override bool StartNextSample()
        {
            current1DDimension = current2DDimension = 0;
            return base.StartNextSample();
        }

        public override bool SetSampleNumber(int sample)
        {
            current1DDimension = current2DDimension = 0;
            return base.SetSampleNumber(sample);
        }

        public override float Get1D()
        {
            // Progressively consume sampled values
            if (current1DDimension < Samples1D.Count)
                return Samples1D[current1DDimension++][CurrentPixelSampleIndex];
            // Return random values when exceeding the allocated values
            else
                return (float)Random.NextDouble();
        }

        public override Point2<float> Get2D()
        {
            // Progressively consume sampled values
            if (current2DDimension < Samples2D.Count)
                return Samples2D[current2DDimension++][CurrentPixelSampleIndex];
            // Return random values when exceeding the allocated values
            else
                return new Point2<float>((float)Random.NextDouble() * 0.999f, (float)Random.NextDouble() * 0.999f);
        }
    }
}
