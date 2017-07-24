namespace pbrt.core
{
    public abstract class Integrator
    {
        public abstract void Render(Scene scene, Window window = null);
    }
}
