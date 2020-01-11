using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace netcore3_simple_game_engine
{
    public struct BufferData2dPlain
    {
        public Vertex2dPlain[] Vertices;
        public uint[] Indices;

        public static BufferData2dPlain PlainSquare(Color4 col)
        {
            return new BufferData2dPlain {
                Vertices = new Vertex2dPlain[] {
                    new Vertex2dPlain {
                        Position = new Vector2d(0, 0),
                        Colour = col
                    },
                    new Vertex2dPlain {
                        Position = new Vector2d(1, 0),
                        Colour = col
                    },
                    new Vertex2dPlain {
                        Position = new Vector2d(1, 1),
                        Colour = col
                    },
                    new Vertex2dPlain {
                        Position = new Vector2d(0, 1),
                        Colour = col
                    }
                },
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData2dPlain PlainRectangle(float width, float height, Color4 col)
        {
            BufferData2dPlain square = PlainSquare(col);

            return new BufferData2dPlain {
                Vertices = square.Vertices.Select(
                    v => new Vertex2dPlain {
                        Position = new Vector2d(
                            v.Position.X * width,
                            v.Position.Y * width
                        ),
                        Colour = v.Colour
                    }
                )
                .ToArray(),
                Indices = square.Indices
            };
        }
    }
}

//                     new Vertex3d(new Vector4( -width/2, -height/2, 0.0f, 1.0f), col),
//                     new Vertex3d(new Vector4(  width/2, -height/2, 0.0f, 1.0f), col),
//                     new Vertex3d(new Vector4(  width/2,  height/2, 0.0f, 1.0f), col),
//                     new Vertex3d(new Vector4( -width/2,  height/2, 0.0f, 1.0f), col),