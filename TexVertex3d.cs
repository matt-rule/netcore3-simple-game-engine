using OpenTK;
using OpenTK.Graphics;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TexVertex3d
    {
        // we use a vector4 to allow it to be multiplied with 4x4 matrices
        public Vector4 Position;
        public Color4 Colour;
        public Vector2 TexCoord;

        public TexVertex3d(Vector4 position, Color4 color, Vector2 texCoord)
        {
            Position = position;
            Colour = color;
            TexCoord = texCoord;
        }
    }
}