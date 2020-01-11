using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace netcore3_simple_game_engine
{
    public class RenderDataPlain : IRenderData
    {
        public Vertex4Plain[] Vertices;
        public uint[] Indices;
        public int VertexArrayObjectId = -1;
        public int VertexBufferObjectId = -1;
        public int IndexBufferObjectId = -1;
        public string ShaderName;

        public RenderDataPlain(BufferData4Plain bufferData, string shaderName)
        {
            Vertices = bufferData.Vertices;
            Indices = bufferData.Indices;
            ShaderName = shaderName;

            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();
        }

        public void Render(Matrix4 Mvp)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
