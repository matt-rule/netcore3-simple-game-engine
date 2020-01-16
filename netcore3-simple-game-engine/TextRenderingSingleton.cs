using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace netcore3_simple_game_engine
{
    public static class TextRenderingSingleton
    {
        /// <summary>
        /// The values held here are exclusively to do with the quad geometry, not textures.
        /// </summary>
        public static Vertex4Textured[] Vertices;
        public static uint[] Indices;
        public static int VertexArrayObjectId = -1;
        public static int VertexBufferObjectId = -1;
        public static int IndexBufferObjectId = -1;
        
        private static void Bind()
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

        public static void Initialise()
        {
            // Get the geometry for a square.
            var bufferData = BufferData4Textured.Rectangle();

            Vertices = bufferData.Vertices;
            Indices = bufferData.Indices;

            VertexArrayObjectId = GL.GenVertexArray();
            VertexBufferObjectId = GL.GenBuffer();
            IndexBufferObjectId = GL.GenBuffer();

            Bind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="frame">Must be >= 0 and < TexCount.</param>
        /// <param name="flip">Flips horizontally.</param>
        public static void RenderFromCorner(SpriteTextureObject spriteTexture, Matrix4 baseMvp, string shaderName, double scale, int frame, bool flip = false)
        {
            Matrix4 finalMatrix = baseMvp;

            finalMatrix = Matrix4.CreateScale((float)scale, (float)scale, 1.0f) * finalMatrix;

            // if (flip)
            // {
            //     finalMatrix =
            //         Matrix4.CreateTranslation(1.0f, 0.0f, 0.0f)
            //         * Matrix4.CreateScale(-1.0f, 1.0f, 1.0f)
            //         * finalMatrix;
            // }
            
            var shaderObj = ShaderObjectSingleton.GetByName(shaderName);                
            GL.UseProgram(shaderObj.ProgramId);
            Bind();
            GL.UniformMatrix4(shaderObj.MatrixShaderLocation, false, ref finalMatrix);

            GL.BindTexture(TextureTarget.Texture2D, spriteTexture.TextureIds[frame]);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindTexture(TextureTarget.Texture2D, -1);
        }

        public static int CorrectIndex(int i)
        {
            char c = '?';
            if (i >= 32 && i <= 126)
                c = (char)i;
            return c - 32;
        }

        public static void RenderString(Matrix4 baseMvp, double x, double y, string text, double size = GameEngineConstants.TEXT_DEFAULT_HEIGHT)
        {
            Matrix4 glyphMatrix = Matrix4.CreateTranslation((float)x, (float)y, 0) * baseMvp;

            for (int i = 0; i < text.Length; ++i)
            {
                RenderFromCorner(
                    TextureObjectSingleton.GetSpriteTextureByName("font"),
                    glyphMatrix,
                    "texture",
                    size,
                    CorrectIndex(text[i])
                );

                float xPosition = (float)(size - (GameEngineConstants.TEXT_KERNING / GameEngineConstants.TEXT_DEFAULT_HEIGHT * size));
                glyphMatrix = Matrix4.CreateTranslation(xPosition, 0, 0) * glyphMatrix;
            }
        }
    }
}