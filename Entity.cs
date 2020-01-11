using OpenTK;
using OpenTK.Graphics.OpenGL4;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public Matrix4 ModelMatrix;
        public IVertexData VertexData;
        public uint[] Indices;
        public int VertexArrayObjectId;
        public int VertexBufferObjectId;
        public int IndexBufferObjectId;
        
        public Entity(string name, Matrix4 modelMatrix, BufferData bufferData, string texture)
        {
            Name = name;
            ModelMatrix = modelMatrix;
            VertexData = new TexVertexData(bufferData.Vertices, texture);
            Indices = bufferData.Indices;
            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();
        }
    }
}
