using OpenTK.Graphics;
using pbrt.core.geometry;
using System;

namespace pbrt.core
{
    public class Film
    {
        public Point2<int> Resolution;
        public float Diagonal;

        // Area that contains all considered pixels
        Bounds2<int> croppedPixelsBounds;

        /*Filter Filter;
        const int filterTableWidth = 16;
        float[] filterTable = new float[filterTableWidth * filterTableWidth];*/

        private struct Pixel
        {
            public float x, y, z;
        }

        private Pixel[] pixels;

        // Crop window specific in normalized coordinates
        public Film(Point2<int> resolution, Bounds2<float> cropWindow/*, Filter filter, float diagonal*/)
        {
            Resolution = resolution;
            //Filter = filter;
            //Diagonal = diagonal * 0.001f; // Millimeters to meters

            croppedPixelsBounds = new Bounds2<int>(
                new Point2<int>(
                    (int)Math.Ceiling(resolution.X * cropWindow.Min.X),
                    (int)Math.Ceiling(resolution.Y * cropWindow.Min.Y)),
                new Point2<int>(
                    (int)Math.Ceiling(resolution.X * cropWindow.Max.X),
                    (int)Math.Ceiling(resolution.Y * cropWindow.Max.Y)));

            pixels = new Pixel[(int)croppedPixelsBounds.Width() * (int)croppedPixelsBounds.Height()];

            /*var offset = 0;
            for (var y = 0; y < filterTableWeight; ++y)
                for (var x = 0; x < filterTableWidth; ++x)
                {
                    var p = new Vector2(
                        (x + 0.5f) * filter.Radius.X / filterTableWidth,
                        (y + 0.5f) * filter.Radius.Y / filterTableWidth);

                    filterTable[offset] = filter.Evaluate(p);
                }*/
        }

        public Bounds2<int> GetSampleBounds()
        {
            // Incorporate pixels out of the crop window so that pixels at the
            // edges have as many samples as the center's
            return new Bounds2<int>(
                (croppedPixelsBounds.Min.ToFloat() + Vector2<float>.One * 0.5f /*- Filter.Radius*/).Floor().ToInt(),
                (croppedPixelsBounds.Max.ToFloat() - Vector2<float>.One * 0.5f /*+ Filter.Radius*/).Ceiling().ToInt());
        }

        public FilmTile GetTile(Bounds2<int> sampleBounds)
        {
            // Account for the radius of the filter
            var halfPixel = Vector2<float>.One * 0.5f;
            var p0 = (sampleBounds.Min.ToFloat() - halfPixel /*- Filter.Radius*/).Ceiling().ToInt();
            var p1 = (sampleBounds.Max.ToFloat() - halfPixel /*+ Filter.Radius*/).Floor().ToInt() + Vector2<int>.One;

            // Account for the overall image bounds
            var tilePixelBounds = Bounds2<int>.Intersect(new Bounds2<int>(p0, p1), croppedPixelsBounds);

            return new FilmTile(tilePixelBounds, /*Filter.Radius*/Vector2<float>.Zero, null/*filterTable*/);
        }

        public Color4 GetPixel(Point2<int> posFilm)
        {
            var x = posFilm.X - croppedPixelsBounds.Min.X;
            var y = posFilm.Y - croppedPixelsBounds.Min.Y;
            var pixel = pixels[y * (int)croppedPixelsBounds.Width() + x];
            return new Color4(pixel.x, pixel.y, pixel.z, 1.0f);
        }

        public void MergeTile(FilmTile tile)
        {
            foreach (var pixel in tile.pixelBounds.IteratePoints())
            {
                var tilePixel = tile.GetPixel(pixel);
                var color = tilePixel.color * (1/50.0f);

                var offset = pixel.Y * (int)croppedPixelsBounds.Width() + pixel.X;
                pixels[offset].x = color.R;
                pixels[offset].y = color.G;
                pixels[offset].z = color.B;
            }
        }
    }
}
