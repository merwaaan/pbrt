using pbrt.core.geometry;
using System;
using static pbrt.core.Film;

namespace pbrt.core
{
    public class FilmTile
    {
        public Bounds2<int> pixelBounds;

        private Filter filter;
        private float[] filterTable;

        private Pixel[] pixels;

        public FilmTile(Bounds2<int> pixelBounds, Filter filter, float[] filterTable)
        {
            this.pixelBounds = pixelBounds;
            this.filter = filter;
            this.filterTable = filterTable;

            pixels = new Pixel[(int)pixelBounds.Width() * (int)pixelBounds.Height()];
        }

        public override string ToString()
        {
            return pixelBounds.ToString();
        }

        public void AddSample(Point2<float> posFilm, Spectrum L, float sampleWeight = 1.0f)
        {
            // Determine the bounds of the pixels that are affected by the new sample

            var posFilmDiscrete = posFilm - new Vector2<float>(0.5f, 0.5f);
            var p0 = (posFilmDiscrete - filter.Radius).Ceiling().ToInt();
            var p1 = (posFilmDiscrete + filter.Radius).Floor().ToInt() + Point2<int>.One;
            p0 = Point2<int>.Max(p0, pixelBounds.Min);
            p1 = Point2<int>.Min(p1, pixelBounds.Max);

            // Compute offsets into the filter weight table for each affected pixel

            var filterTableSize = (int)Math.Sqrt(filterTable.Length);

            var ifx = new int[p1.X - p0.X];
            for (var x = p0.X; x < p1.X; ++x)
            {
                var fx = Math.Abs((x - posFilmDiscrete.X) * filter.InvRadius.X * filterTableSize);
                ifx[x - p0.X] = (int)Math.Min(Math.Floor(fx), filterTableSize - 1);
            }

            var ify = new int[p1.Y - p0.Y];
            for (var y = p0.Y; y < p1.Y; ++y)
            {
                var fy = Math.Abs((y - posFilmDiscrete.Y) * filter.InvRadius.Y * filterTableSize);
                ify[y - p0.Y] = (int)Math.Min(Math.Floor(fy), filterTableSize - 1);
            }

            // Add the effective contribution for each pixel

            for (var y = p0.Y; y < p1.Y; ++y)
                for (var x = p0.X; x < p1.X; ++x)
                {
                    // Evaluate the filter value at the current pixel
                    var offset = ify[y - p0.Y] * filterTableSize + ifx[x - p0.X];
                    var filterWeight = filterTable[offset];

                    //
                    var tileOffset = (y - pixelBounds.Min.Y) * (int)pixelBounds.Width() + (x - pixelBounds.Min.X);
                    pixels[tileOffset].contribSum += L * filterWeight * sampleWeight;
                    pixels[tileOffset].filterWeightSum += filterWeight;
                }
        }

        public Pixel GetPixel(Point2<int> posFilm)
        {
            var x = posFilm.X - pixelBounds.Min.X;
            var y = posFilm.Y - pixelBounds.Min.Y;
            return pixels[y * (int)pixelBounds.Width() + x];
        }
    }
}
