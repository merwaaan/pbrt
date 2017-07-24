using pbrt.core;
/*
namespace pbrt.samplers
{
    class GlobalSampler : Sampler
    {
        private int dimension;
        private int intervalSampleIndex;

        public GlobalSampler(int samplesPerPixel)
            : base(samplesPerPixel)
        {

        }

        public abstract int GetIndexForSample(int sample);
        public abstract float SampleDimension(int index, int dimension);

        public override void StartPixel(Vector2 p)
        {
            base.StartPixel(p);

            dimension = 0;
            intervalSampleIndex = GetIndexForSample(0);
        }
    }
}
*/