using pbrt.core.geometry;

namespace pbrt.core
{
    public class FilmTile
    {
        public Bounds2<int> pixelBounds;

        private Vector2<float> filterRadius, invFilterRadius;
        private float[] filterTable;

        public struct FilmTilePixel
        {
            public Spectrum color; // TODO temp
            //public Spectrum contribSum ;
            public float filterWeightSum;
        }

        private FilmTilePixel[] pixels;

        public FilmTile(Bounds2<int> pixelBounds, Vector2<float> filterRadius, float[] filterTable)
        {
            this.pixelBounds = pixelBounds;
            this.filterRadius = filterRadius;
            this.invFilterRadius = new Vector2<float>(1.0f / filterRadius.X, 1.0f / filterRadius.Y);
            this.filterTable = filterTable;

            pixels = new FilmTilePixel[(int)pixelBounds.Width() * (int)pixelBounds.Height()];
        }

        public override string ToString()
        {
            return pixelBounds.ToString();
        }

        public void AddSample(Point2<float> posFilm, Spectrum color, float sampleWeight = 1.0f)
        {
            var posTile = posFilm.ToInt() - pixelBounds.Min;
            pixels[posTile.Y * (int)pixelBounds.Width() + posTile.X].color = color;
        }

        public FilmTilePixel GetPixel(Point2<int> posFilm)
        {
            var x = posFilm.X - pixelBounds.Min.X;
            var y = posFilm.Y - pixelBounds.Min.Y;
            return pixels[y * (int)pixelBounds.Width() + x];
        }
    }
}
