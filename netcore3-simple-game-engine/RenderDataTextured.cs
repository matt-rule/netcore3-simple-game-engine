using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    public class RenderDataTextured: IRenderData
    {
        public Vertex4Textured[] Vertices;
        public uint[] Indices;
        public int VertexArrayObjectId = -1;
        public int VertexBufferObjectId = -1;
        public int IndexBufferObjectId = -1;

        // Identifies a texture object by name, distinct from filename.
        public string TextureName;
        public string ShaderName;

        public RenderDataTextured(BufferData4Textured bufferData, string textureName, string shaderName)
        {
            Vertices = bufferData.Vertices;
            Indices = bufferData.Indices;
            TextureName = textureName;
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

            // create vertex buffer
            GL.NamedBufferStorage(
                VertexBufferObjectId,
                6 * sizeof(float) * Vertices.Length,
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
            GL.VertexArrayAttribFormat(VertexArrayObjectId, 1, 2, VertexAttribType.Float, false, Marshal.SizeOf<Vector4>());
            
            GL.VertexArrayVertexBuffer(VertexArrayObjectId, 0, VertexBufferObjectId, IntPtr.Zero, 6 * sizeof(float));

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
            var textureObject = TextureObjectSingleton.GetSingleTextureByName(TextureName);
            
            var shaderObj = ShaderObjectSingleton.GetByName(ShaderName);                
            GL.UseProgram(shaderObj.ProgramId);
            Bind();
            GL.UniformMatrix4(shaderObj.MatrixShaderLocation, false, ref Mvp);

            GL.BindTexture(TextureTarget.Texture2D, textureObject.TextureId);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindTexture(TextureTarget.Texture2D, -1);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}