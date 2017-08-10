namespace pbrt.core
{
    public abstract class Integrator
    {
        public abstract void Render(Scene scene, Camera camera, Window window = null);

        public virtual void Preprocess(Scene scene)
        {
        }
    }
}
