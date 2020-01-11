using OpenTK;
using OpenTK.Graphics;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2Textured
    {
        public Vector2 Position;
        public Vector2 TexCoord;
    }
}