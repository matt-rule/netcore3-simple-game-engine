using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace netcore3_simple_game_engine
{
    /// <summary>
    /// Instances of this class should only be created after OpenGL initialisation.
    /// </summary>
    public class TextureObject: IDisposable
    {
        public Bitmap Bitmap = null;

        // References the texture inside OpenGL.
        public int TextureId = -1;

        // Name used to identify this texture (not a filename).
        public string Name;

        public TextureObject(string filename)
        {
            GL.GenTextures(1, out TextureId);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            BitmapData data = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            Bitmap.UnlockBits(data);
        }
        public void Dispose()
        {
            GL.DeleteTextures(1, ref TextureId);
        }
    }
}