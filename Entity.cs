using OpenTK;
using OpenTK.Graphics.OpenGL4;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public Matrix4 ModelMatrix;
        public Vertex4Plain[] Vertices;
        public uint[] Indices;
        public int VertexArrayObjectId;
        public int VertexBufferObjectId;
        public int IndexBufferObjectId;
        
        public Entity(string name, Matrix4 modelMatrix, BufferData bufferData)
        {
            Name = name;
            ModelMatrix = modelMatrix;
            Vertices = bufferData.Vertices;
            Indices = bufferData.Indices;
            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();
        }
    }
}
