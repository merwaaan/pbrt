using pbrt.core.geometry;
using static pbrt.core.Film;

namespace pbrt.core
{
    public class FilmTile
    {
        public Bounds2<int> pixelBounds;

        private Vector2<float> filterRadius, invFilterRadius;
        private float[] filterTable;

        private Pixel[] pixels;

        public FilmTile(Bounds2<int> pixelBounds, Vector2<float> filterRadius, float[] filterTable)
        {
            this.pixelBounds = pixelBounds;
            this.filterRadius = filterRadius;
            this.invFilterRadius = new Vector2<float>(1.0f / filterRadius.X, 1.0f / filterRadius.Y);
            this.filterTable = filterTable;

            pixels = new Pixel[(int)pixelBounds.Width() * (int)pixelBounds.Height()];
        }

        public override string ToString()
        {
            return pixelBounds.ToString();
        }

        public void AddSample(Point2<float> posFilm, Spectrum contrib, float sampleWeight = 1.0f)
        {
            var posTile = posFilm.ToInt() - pixelBounds.Min;
            var tileOffset = posTile.Y * (int)pixelBounds.Width() + posTile.X;
            pixels[tileOffset].contribSum = contrib;
            pixels[tileOffset].filterWeightSum = 1 * sampleWeight; // TODO add filter weight when implementing filters
        }

        public Pixel GetPixel(Point2<int> posFilm)
        {
            var x = posFilm.X - pixelBounds.Min.X;
            var y = posFilm.Y - pixelBounds.Min.Y;
            return pixels[y * (int)pixelBounds.Width() + x];
        }
    }
}
