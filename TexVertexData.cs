using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public class TexVertexData: IVertexData, IDisposable
    {
        public TexVertex3d[] Vertices;
        public Bitmap Bitmap = null;
        public int Id = -1;

        public TexVertexData(Vertex3d[] vertices, string textureFileName)
        {
            Bitmap = new Bitmap(textureFileName);
            
            Vertices = vertices.Select(x => new TexVertex3d
            {
                Position = x.Position,
                Colour = x.Colour,
                TexCoord = x.Position.Xy
            }).ToArray();
        }

        public void Initialise()
        {
            GL.GenTextures(1, out Id);
            GL.BindTexture(TextureTarget.Texture2D, Id);

            BitmapData data = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            Bitmap.UnlockBits(data);
        }

        public void Render()
        {
            GL.BindTexture(TextureTarget.Texture2D, Id);

        //     // GL.Begin(PrimitiveType.Quads);
        //     // {
        //     //     GL.TexCoord2(0.0f, 1.0f);
        //     //     GL.Vertex2(0.0, 0.0);

        //     //     GL.TexCoord2(1.0f, 1.0f);
        //     //     GL.Vertex2(1.0, 0.0);

        //     //     GL.TexCoord2(1.0f, 0.0f);
        //     //     GL.Vertex2(1.0, 1.0);

        //     //     GL.TexCoord2(0.0f, 0.0f);
        //     //     GL.Vertex2(0.0, 1.0);
        //     // }

            GL.BindTexture(TextureTarget.Texture2D, -1);
        }

        public void Dispose()
        {
            GL.DeleteTextures(1, ref Id);
        }
    }
}