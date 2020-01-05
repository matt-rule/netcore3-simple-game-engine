using OpenTK;
using OpenTK.Graphics;

namespace netcore3_simple_game_engine
{
    public struct Vertex3d
    {
        // we use a vector4 to allow it to be multiplied with 4x4 matrices
        private readonly Vector4 Position;
        private readonly Color4 Colour;

        public Vertex3d(Vector4 position, Color4 color)
        {
            Position = position;
            Colour = color;
        }
    }
}