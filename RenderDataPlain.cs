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

            Bind();
        }

        private void Bind()
        {
            GL.BindVertexArray(VertexArrayObjectId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObjectId);

            ErrorCode error = GL.GetError();
            error = GL.GetError();
            // create vertex buffer
            GL.NamedBufferStorage(
                VertexBufferObjectId,
                8 * sizeof(float) * Vertices.Length,
                Vertices,
                BufferStorageFlags.MapWriteBit);

            error = GL.GetError();
            // position attribute
            GL.VertexArrayAttribBinding(VertexArrayObjectId, 0, 0);
            error = GL.GetError();
            GL.EnableVertexArrayAttrib(VertexArrayObjectId, 0);
            error = GL.GetError();
            // VertexArrayAttribFormat is the equivalent of glVertexAttribPointer
            GL.VertexArrayAttribFormat(VertexArrayObjectId, 0, 4, VertexAttribType.Float, false, 0);
            error = GL.GetError();

            // colour attribute
            // position precedes this; offset by its size
            GL.VertexArrayAttribBinding(VertexArrayObjectId, 1, 0);
            GL.EnableVertexArrayAttrib(VertexArrayObjectId, 1);
            GL.VertexArrayAttribFormat(VertexArrayObjectId, 1, 4, VertexAttribType.Float, false, Marshal.SizeOf<Vector4>());
            
            GL.VertexArrayVertexBuffer(VertexArrayObjectId, 0, VertexBufferObjectId, IntPtr.Zero, 8 * sizeof(float));

            // index buffer object
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferObjectId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * Indices.Length, Indices, BufferUsageHint.StaticDraw);
        }

        private void Unbind()
        {
            GL.DeleteVertexArray(VertexArrayObjectId);
            GL.DeleteBuffer(VertexBufferObjectId);
            GL.DeleteBuffer(IndexBufferObjectId);
        }

        public void Render(Matrix4 Mvp)
        {
            var shaderObj = ShaderObjectSingleton.GetByName(ShaderName);                
            GL.UseProgram(shaderObj.ProgramId);
            Bind();
            GL.UniformMatrix4(shaderObj.MatrixShaderLocation, false, ref Mvp);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}
