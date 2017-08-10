using pbrt.core.geometry;
using System.Collections.Generic;
using static pbrt.Camera;

namespace pbrt.core
{
    public abstract class Sampler
    {
        public readonly int SamplesPerPixel;

        protected Point2<int> CurrentPixel;
        protected int CurrentPixelSampleIndex;

        private List<float[]> sample1DArrays = new List<float[]>();
        private int array1DIndex;

        private List<Point2<float>[]> sample2DArrays = new List<Point2<float>[]>();
        private int array2DIndex;

        public Sampler(int samplesPerPixel)
        {
            SamplesPerPixel = samplesPerPixel;
        }

        public abstract Sampler Clone(int seed);

        // Returns sample values for the next dimension/the next two dimensions
        public abstract float Get1D();
        public abstract Point2<float> Get2D();

        // Initiates sampling for the given pixel
        public virtual void StartPixel(Point2<int> pixel)
        {
            CurrentPixel = pixel;
            CurrentPixelSampleIndex = 0;
            array1DIndex = array2DIndex = 0;
        }

        // Moves on to the next sample for the current pixel
        public virtual bool StartNextSample()
        {
            ++CurrentPixelSampleIndex;
            array1DIndex = array2DIndex = 0;
            return CurrentPixelSampleIndex < SamplesPerPixel;
        }

        // Sets the index of the sample in the current pixel to generate
        // (for algorithms that need to "jump" back and forth between different pixels)
        public virtual bool SetSampleNumber(int sample)
        {
            CurrentPixelSampleIndex = sample;
            array1DIndex = array2DIndex = 0;
            return CurrentPixelSampleIndex < SamplesPerPixel;
        }

        // Request the sampler to prepare 1D/2D sample arrays ahead of time

        public void Request1DArray(int n)
        {
            sample1DArrays.Add(new float[n * SamplesPerPixel]);
        }

        public void Request2DArray(int n)
        {
            sample2DArrays.Add(new Point2<float>[n * SamplesPerPixel]);
        }

        // Get the samples from the requested arrays

        public float[] Get1DArray(int n)
        {
            return null;
            /*
            if (array1DIndex >= sample1DArrays.Count)
                return null;

            return sample1DArrays[array1DIndex++][CurrentPixelSampleIndex * n];
        */}

        public Point2<float>[] Get2DArray(int n)
        {
            return null;
            /*
            if (array2DIndex >= sample2DArrays.Count)
                return null;

            return sample2DArrays[array2DIndex++][CurrentPixelSampleIndex * n];
        */}

        // Adjust a sample count depending on the specificities of a sampler's implementation
        // (the returned value should be used when requesting samples)
        public virtual int RoundCount(int n)
        {
            return n;
        }

        // Convenience function to initialize camera samples for the given pixel
        public CameraSample GetCameraSample(Point2<int> posRaster)
        {
            CameraSample cs = new CameraSample();
            cs.PosFilm = posRaster.ToFloat() + Get2D();
            cs.Time = Get1D();
            cs.PosLens = Get2D();
            return cs;
        }
    }
}
