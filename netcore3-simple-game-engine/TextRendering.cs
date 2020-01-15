using OpenTK;

namespace netcore3_simple_game_engine
{
    public static class TextRendering
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="frame">Must be >= 0 and < TexCount.</param>
        /// <param name="flip">Flips horizontally.</param>
        public static void GlRenderFromCorner(Matrix4 baseMvp, double scale, int frame, bool flip = false)
        {
            GL.PushMatrix();
            {
                GL.Scale(scale, scale, 1.0);
                if (flip)
                {
                    GL.Translate(1.0, 0.0, 0.0);
                    GL.Scale(-1.0, 1.0, 1.0);
                }
                GL.BindTexture(TextureTarget.Texture2D, Ids[frame]);

                GL.Begin(PrimitiveType.Quads);
                {
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex2(0.0, 0.0);

                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(1.0, 0.0);

                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(1.0, 1.0);

                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(0.0, 1.0);
                }
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, -1);
            }
            GL.PopMatrix();
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
            Matrix4 stringMatrix = Matrix4.CreateTranslation((float)x, (float)y, 0) * baseMvp;

            for (int i = 0; i < text.Length; ++i)
            {
                if (spriteTexObjects.TryGetValue(GameEngineConstants.TEX_ID_SPRITE_FONT, out SpriteTexObject spriteFontTexObject))
                    spriteFontTexObject.GlRenderFromCorner(size, CorrectIndex(text[i]));

                float xPosition = (float)(size - (GameEngineConstants.TEXT_KERNING / GameEngineConstants.TEXT_DEFAULT_HEIGHT * size));
                Matrix4 glyphMatrix = 
                GL.Translate(, 0, 0);
            }
        }
    }
}