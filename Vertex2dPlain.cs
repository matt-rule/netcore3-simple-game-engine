using OpenTK;
using OpenTK.Graphics;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2dPlain
    {
        public Vector2d Position;
        public Color4 Colour;
    }
}