using OpenTK;
using OpenTK.Graphics;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex4Plain
    {
        public Vector4 Position;
        public Color4 Colour;
    }
}