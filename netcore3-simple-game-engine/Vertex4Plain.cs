using OpenTK;
using OpenTK.Graphics;
using System.Runtime.InteropServices;

namespace netcore3_simple_game_engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex4Plain
    {
        // we use a vector4 to allow it to be multiplied with 4x4 matrices in the shader
        public Vector4 Position;
        public Color4 Colour;
    }
}