using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace netcore3_simple_game_engine
{
    /// <summary>
    /// Instances of this class should only be created after OpenGL initialisation.
    /// </summary>
    public class SpriteTextureObject: IDisposable
    {
        public Bitmap[] Bitmaps = null;

        // References the texture inside OpenGL.
        public int[] TextureIds = null;

        // Name used to identify this texture (not a filename).
        public string Name;
        public int TextureCount = 0;

        public SpriteTextureObject(string name, string filename, int textureWidth, int frameCount)
        {
            Name = name;
            TextureCount = frameCount;
            Bitmaps = new Bitmap[frameCount];
            var wholeFileBitmap = new Bitmap(filename);

            foreach (int frame in Enumerable.Range(0, frameCount))
            {
                Rectangle cropRect = new Rectangle(frame * textureWidth, 0, textureWidth, textureWidth);
                Bitmaps[frame] = new Bitmap(cropRect.Width, cropRect.Height);
                
                Bitmap target = Bitmaps[frame];

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(
                        wholeFileBitmap,
                        new Rectangle(0, 0, target.Width, target.Height),
                        cropRect,
                        GraphicsUnit.Pixel
                    );
                }
            }

            TextureIds = new int[TextureCount];
            GL.GenTextures(TextureCount, TextureIds);

            foreach (int frame in Enumerable.Range(0, TextureCount))
            {
                Bitmap frameBitmap = Bitmaps[frame];

                GL.BindTexture(TextureTarget.Texture2D, TextureIds[frame]);

                BitmapData bitmapData = frameBitmap.LockBits(new Rectangle(0, 0, frameBitmap.Width, frameBitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

                frameBitmap.UnlockBits(bitmapData);
            }
        }
        public void Dispose()
        {
            GL.DeleteTextures(TextureCount, TextureIds);
        }
    }
}