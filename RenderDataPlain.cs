using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace netcore3_simple_game_engine
{
    public class RenderDataPlain : IRenderData
    {
        public Vertex2Plain[] Vertices;
        public uint[] Indices;
        public int VertexArrayObjectId = -1;
        public int VertexBufferObjectId = -1;
        public int IndexBufferObjectId = -1;

        public RenderDataPlain()
        {
            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();
        }

        private void Bind()
        {
            GL.BindVertexArray(VertexArrayObjectId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObjectId);

            // create vertex buffer
            GL.NamedBufferStorage(
                VertexBufferObjectId,
                8 * sizeof(float) * Vertices.Length,
                Vertices,
                BufferStorageFlags.MapWriteBit);

            // position attribute
            GL.VertexArrayAttribBinding(VertexArrayObjectId, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArrayObjectId, 0);
            // VertexArrayAttribFormat is the equivalent of glVertexAttribPointer
            GL.VertexArrayAttribFormat(VertexArrayObjectId, 0, 4, VertexAttribType.Float, false, 0);

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
            if (VertexArrayObjectId != -1)
            {
                GL.DeleteVertexArray(VertexArrayObjectId);
                VertexArrayObjectId = -1;
            }
            if (VertexBufferObjectId != -1)
            {
                GL.DeleteBuffer(VertexBufferObjectId);
                VertexBufferObjectId = -1;
            }
            if (IndexBufferObjectId != -1)
            {
                GL.DeleteBuffer(IndexBufferObjectId);
                IndexBufferObjectId = -1;
            }
        }

        public void Render()
        {
            Bind();
            Unbind();
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}
