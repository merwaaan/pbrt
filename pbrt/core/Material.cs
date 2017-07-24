namespace pbrt.core
{
    public interface Material
    {
        void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes);
    }
}
