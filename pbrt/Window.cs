using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using pbrt.core;
using pbrt.core.geometry;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace pbrt
{
    public class Window : GameWindow
    {
        private Integrator integrator;
        private Scene scene;

        private static int WindowId;
        private int id;

        private byte[] bitmap;
        private int texture;

        public Window(Integrator integrator, Scene scene)
            : base(Program.Width, Program.Height, GraphicsMode.Default, $"PBRT - {integrator}",
                  GameWindowFlags.Default, DisplayDevice.Default,
                  3, 0, GraphicsContextFlags.ForwardCompatible)
        {
            this.integrator = integrator;
            this.scene = scene;

            bitmap = new byte[Width * Height * 4];
                        
            // Setup the output texture
            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            id = WindowId++;
            Location = new System.Drawing.Point(Width * id, 0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Start rendering
            Task.Factory.StartNew(() => integrator.Render(scene, this));
        }

        private static readonly object renderingLock = new object();

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1.0f, 1.0f);

            GL.End();

            SwapBuffers();
        }

        // Mark tile as being rendered
        public void MarkTile(Bounds2<int> tileBounds)
        {
            for (var y = tileBounds.Min.Y; y < tileBounds.Max.Y; ++y)
                for (var x = tileBounds.Min.X; x < tileBounds.Max.X; ++x)
                    bitmap[(y * Width + x) * 4] = 0xFF;
        }

        public void UnmarkTile(Bounds2<int> tileBounds)
        {
            for (var y = tileBounds.Min.Y; y < tileBounds.Max.Y; ++y)
                for (var x = tileBounds.Min.X; x < tileBounds.Max.X; ++x)
                    bitmap[(y * Width + x) * 4] = 0;
        }

        // Update the texture with a portion of the given film.
        public void UpdateTileFromFilm(Bounds2<int> tileBounds, Film film)
        {
            foreach (var posFilm in tileBounds.IteratePoints())
            {
                var pixel = film.GetPixel(posFilm);
                var color = pixel.contribSum / pixel.filterWeightSum;

                var offset = (posFilm.Y * Width + posFilm.X) * 4; // BGRA
                bitmap[offset + 0] = (byte)Math.Min(255, color.B * 255);
                bitmap[offset + 1] = (byte)Math.Min(255, color.G * 255);
                bitmap[offset + 2] = (byte)Math.Min(255, color.R * 255);
            }
        }
    }
}
