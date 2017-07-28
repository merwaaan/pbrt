using pbrt.core;
using pbrt.core.geometry;
using System;
using System.Threading.Tasks;

namespace pbrt.integrators
{
    public abstract class SamplerIntegrator : Integrator
    {
        protected Sampler Sampler;
        
        public SamplerIntegrator(Sampler sampler)
        {
            Sampler = sampler;
        }

        public abstract Spectrum Li(RayDifferential ray, Scene scene, Camera camera, Sampler sampler, int depth = 0);

        protected virtual void Preprocess()
        {
        }

        public override void Render(Scene scene, Camera camera, Window window = null)
        {
            Preprocess();

            // Compute the number of tiles to use

            var sampleBounds = camera.Film.GetSampleBounds();
            var sampleExtent = sampleBounds.Diagonal();

            var nTiles = new Vector2<int>(
                (sampleExtent.X + Program.TileSize - 1) / Program.TileSize,
                (sampleExtent.Y + Program.TileSize - 1) / Program.TileSize);

            // Render tiles in parallel

            //Parallel.For(0, nTiles.X * nTiles.Y, tile =>
            Parallel.For(0, nTiles.X * nTiles.Y, new ParallelOptions { MaxDegreeOfParallelism = Program.ThreadCount }, tile =>
            {
                var tileX = tile % nTiles.X;
                var tileY = tile / nTiles.X;

                // Get a sampler instance for the current tile
                var seed = tileY * nTiles.X + tileX;
                var tileSampler = Sampler.Clone(seed);

                // Compute the extent of pixels to be sampled in this tile
                var x0 = sampleBounds.Min.X + tileX * Program.TileSize;
                var x1 = Math.Min(x0 + Program.TileSize, sampleBounds.Max.X);
                var y0 = sampleBounds.Min.Y + tileY * Program.TileSize;
                var y1 = Math.Min(y0 + Program.TileSize, sampleBounds.Max.Y);
                var tileBounds = new Bounds2<int>(new Point2<int>(x0, y0), new Point2<int>(x1, y1));

                var filmTile = camera.Film.GetTile(tileBounds);
                window?.MarkTile(tileBounds);

                // Loop over the pixels in the tile
                foreach (var pixel in tileBounds.IteratePoints())
                {
                    tileSampler.StartPixel(pixel);
                    do
                    {
                        var cameraSample = tileSampler.GetCameraSample(pixel);

                        // Generate a camera ray for the current sample
                        var rayWeight = camera.GenerateRayDifferential(cameraSample, out RayDifferential ray);
                        ray.ScaleDifferentials((float)(1.0f / Math.Sqrt(tileSampler.SamplesPerPixel)));

                        // Evaluate radiance along the camera ray
                        var L = Spectrum.Zero;
                        if (rayWeight > 0)
                            L = Li(ray, scene, camera, tileSampler);

                        // TODO check invalid radiance

                        // Add the ray's contribution to the image
                        filmTile.AddSample(cameraSample.PosFilm, L, rayWeight);

                    } while (tileSampler.StartNextSample());
                }

                // Merge the tile into the film
                camera.Film.MergeTile(filmTile);

                //Thread.Sleep(100);
                window?.UnmarkTile(tileBounds);
                window?.UpdateTileFromFilm(tileBounds, camera.Film);
            });
        }
    }
}
