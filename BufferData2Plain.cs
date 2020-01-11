using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace netcore3_simple_game_engine
{
    public class BufferData2Plain
    {
        public Vertex4Plain[] Vertices;
        public uint[] Indices;

        public static BufferData2Plain Rectangle(Color4 col, float width = 1, float height = 1)
        {
            return new BufferData2Plain {
                Vertices = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(width, 0),
                    new Vector2(width, height),
                    new Vector2(0, height)
                }
                .Select(v => new Vertex4Plain {
                    Position = new Vector4(v.X, v.Y, 0.0f, 1.0f),
                    Colour = col
                })
                .ToArray(),
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData2Plain CentredRectangle(Color4 col, float width, float height)
        {
            return new BufferData2Plain {
                Vertices = new Vector2[] {
                    new Vector2(-width/2, -height/2),
                    new Vector2(width/2, -height/2),
                    new Vector2(width/2, height/2),
                    new Vector2(-width/2, height/2)
                }
                .Select(v => new Vertex4Plain {
                    Position = new Vector4(v.X, v.Y, 0.0f, 1.0f),
                    Colour = col
                })
                .ToArray(),
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData2Plain UnitCircle(Color4 col, int vertices, float radius)
        {
            return new BufferData2Plain {
                Vertices = new Vertex4Plain[] {
                    new Vertex4Plain{
                        Position = Vector4.Zero,
                        Colour = col
                    }
                }
                .Concat (
                    Enumerable.Range(1, vertices)
                    .Select(vertex => 2*Math.PI*vertex/vertices)
                    .Select(angleRadians => new Vertex4Plain{
                            Position = new Vector4(
                                (float)(radius*Math.Cos(angleRadians)),
                                (float)(radius*Math.Sin(angleRadians)),
                                0.0f,
                                1.0f
                            ),
                            Colour = col
                        }
                    )
                )
                .ToArray(),
                Indices = Enumerable.Range(0, vertices)
                .SelectMany(x => new uint[]
                    {
                        0,
                        (uint)((x + 1) <= vertices ? (x + 1) : ((x + 1) - vertices)),
                        (uint)((x + 2) <= vertices ? (x + 2) : ((x + 2) - vertices))
                    }
                )
                .ToArray()
            };
        }
    }
}