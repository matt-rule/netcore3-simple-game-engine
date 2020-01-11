using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public class RenderDataTextured: IRenderData
    {
        public Vertex2Textured[] Vertices;
        public uint[] Indices;
        // Identifies a texture object by name, distinct from filename.
        public string textureName;

        public void Render()
        {
            var textureObject = TextureObjectSingleton.GetByName(textureName);

            GL.BindTexture(TextureTarget.Texture2D, textureObject.TextureId);

            // TODO: A call to GLDrawArrays or something here.

            GL.BindTexture(TextureTarget.Texture2D, -1);
        }
    }
}