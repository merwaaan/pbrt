namespace pbrt.samplers
{
    public class RandomSampler : PixelSampler
    {
        public RandomSampler(int samplesPerPixel)
            : base(samplesPerPixel, 0)
        {
        }
    }
}
