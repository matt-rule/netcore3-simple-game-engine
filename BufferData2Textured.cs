using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace netcore3_simple_game_engine
{
    public class BufferData2Textured
    {
        public Vertex4Textured[] Vertices;
        public uint[] Indices;

        public static BufferData2Textured Rectangle(Color4 col, float width = 1, float height = 1)
        {
            return new BufferData2Textured {
                Vertices = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                }
                .Select(v => new Vertex4Textured {
                    Position = new Vector4(v.X * width, v.Y * height, 0.0f, 1.0f),
                    TexCoord = v
                })
                .ToArray(),
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData2Textured CentredRectangle(Color4 col, float width, float height)
        {
            return new BufferData2Textured {
                Vertices = new Vector2[] {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1)
                }
                .Select(v => new Vertex4Textured {
                    Position = new Vector4((v.X-0.5f) * width, (v.Y-0.5f) * height, 0.0f, 1.0f),
                    TexCoord = v
                })
                .ToArray(),
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData2Textured UnitCircle(Color4 col, int vertices, float radius)
        {
            var UnitCircleVertices =
                new Vector2[]{
                    new Vector2(0,0)
                }
                .Concat(
                    Enumerable.Range(1, vertices)
                    .Select(vertex => 2*Math.PI*vertex/vertices)
                    .Select(angleRadians => new Vector2(
                            (float)(radius*Math.Cos(angleRadians)),
                            (float)(radius*Math.Sin(angleRadians))
                        )
                    )
                )
                .ToArray();

            return new BufferData2Textured {
                Vertices = UnitCircleVertices.Select(
                    v => new Vertex4Textured {
                        Position = new Vector4(v.X, v.Y, 0.0f, 1.0f),
                        TexCoord = new Vector2(v.X/2 + 0.5f, v.Y/2 + 0.5f)
                    }
                )
                .ToArray(),
                Indices = Enumerable.Range(1, vertices)
                .SelectMany(x => new uint[]
                    {
                        0,
                        (uint)(x <= vertices ? x : (x - vertices)),
                        (uint)((x + 1) <= vertices ? (x + 1) : ((x + 1) - vertices))
                    }
                )
                .ToArray()
            };
        }
    }
}