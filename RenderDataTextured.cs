using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace netcore3_simple_game_engine
{
    public class RenderDataTextured: IRenderData
    {
        public Vertex4Textured[] Vertices;
        public uint[] Indices;
        // Identifies a texture object by name, distinct from filename.
        public string textureName;

        public void Render(Matrix4 Mvp)
        {
            var textureObject = TextureObjectSingleton.GetByName(textureName);

            GL.BindTexture(TextureTarget.Texture2D, textureObject.TextureId);

            // TODO: A call to GLDrawArrays or something here.

            GL.BindTexture(TextureTarget.Texture2D, -1);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}