using OpenTK;
using OpenTK.Graphics.OpenGL4;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public Matrix4 ModelMatrix;
        public IRenderData RenderData;
        public int VertexArrayObjectId;
        public int VertexBufferObjectId;
        public int IndexBufferObjectId;
        
        public Entity(string name, Matrix4 modelMatrix, IRenderData renderData, string texture)
        {
            Name = name;
            ModelMatrix = modelMatrix;
            RenderData = renderData;
            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();
        }
    }
}
