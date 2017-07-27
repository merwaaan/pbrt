using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public class Film
    {
        public Point2<int> Resolution;
        //public float Diagonal;

        public Filter Filter { get; private set; }

        // Area to render
        private Bounds2<int> croppedPixelsBounds;

        // Cached filter values
        private const int filterTableSize = 16;
        private float[] filterTable = new float[filterTableSize * filterTableSize];

        public struct Pixel
        {
            public Spectrum contribSum;
            public float filterWeightSum;
        }

        private Pixel[] pixels;

        // Crop window specific in normalized coordinates
        public Film(Point2<int> resolution, Bounds2<float> cropWindow, Filter filter/*, float diagonal*/)
        {
            Resolution = resolution;
            Filter = filter;
            //Diagonal = diagonal * 0.001f; // Millimeters to meters

            croppedPixelsBounds = new Bounds2<int>(
                new Point2<int>(
                    (int)Math.Ceiling(resolution.X * cropWindow.Min.X),
                    (int)Math.Ceiling(resolution.Y * cropWindow.Min.Y)),
                new Point2<int>(
                    (int)Math.Ceiling(resolution.X * cropWindow.Max.X),
                    (int)Math.Ceiling(resolution.Y * cropWindow.Max.Y)));

            pixels = new Pixel[Resolution.X * Resolution.Y];

            // Precompute filter weights
            var offset = 0;
            for (var y = 0; y < filterTableSize; ++y)
                for (var x = 0; x < filterTableSize; ++x, ++offset)
                {
                    var p = new Point2<float>(
                        (x + 0.5f) * filter.Radius.X / filterTableSize,
                        (y + 0.5f) * filter.Radius.Y / filterTableSize);

                    filterTable[offset] = filter.Evaluate(p);
                }
        }

        public Bounds2<int> GetSampleBounds()
        {
            // Incorporate pixels out of the crop window so that pixels at the
            // edges have as many samples as the center's
            return new Bounds2<int>(
                (croppedPixelsBounds.Min.ToFloat() + Vector2<float>.One * 0.5f - Filter.Radius).Floor().ToInt(),
                (croppedPixelsBounds.Max.ToFloat() - Vector2<float>.One * 0.5f + Filter.Radius).Ceiling().ToInt());
        }

        public FilmTile GetTile(Bounds2<int> sampleBounds)
        {
            // Account for the radius of the filter
            var halfPixel = Vector2<float>.One * 0.5f;
            var p0 = (sampleBounds.Min.ToFloat() - halfPixel - Filter.Radius).Ceiling().ToInt();
            var p1 = (sampleBounds.Max.ToFloat() - halfPixel + Filter.Radius).Floor().ToInt() + Vector2<int>.One;

            // Account for the overall image bounds
            var tilePixelBounds = Bounds2<int>.Intersect(new Bounds2<int>(p0, p1), croppedPixelsBounds);

            return new FilmTile(tilePixelBounds, Filter, filterTable);
        }

        public Pixel GetPixel(Point2<int> posFilm)
        {
            var x = posFilm.X - croppedPixelsBounds.Min.X;
            var y = posFilm.Y - croppedPixelsBounds.Min.Y;
            return pixels[posFilm.Y * Resolution.X + posFilm.X];
        }

        public void MergeTile(FilmTile tile)
        {
            foreach (var pixel in tile.pixelBounds.IteratePoints())
            {
                var tilePixel = tile.GetPixel(pixel);
                
                var offset = pixel.Y * Resolution.X + pixel.X;
                pixels[offset].contribSum += tilePixel.contribSum;
                pixels[offset].filterWeightSum += tilePixel.filterWeightSum;
            }
        }
    }
}
