using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public static class Geometry
    {
        public static BufferData Rectangle(float width, float height, Color4 col)
        {
            return new BufferData {
                Vertices = new Vertex4Plain[] {
                    new Vertex4Plain{Position=new Vector4( -width/2, -height/2, 0.0f, 1.0f), Colour=col},
                    new Vertex4Plain{Position=new Vector4(  width/2, -height/2, 0.0f, 1.0f), Colour=col},
                    new Vertex4Plain{Position=new Vector4(  width/2,  height/2, 0.0f, 1.0f), Colour=col},
                    new Vertex4Plain{Position=new Vector4( -width/2,  height/2, 0.0f, 1.0f), Colour=col},
                },
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData Circle(int vertices, float radius, Color4 col)
        {
            return new BufferData {
                Vertices = new List<Vertex4Plain> {
                    new Vertex4Plain{Position=new Vector4(0.0f, 0.0f, 0.0f, 1.0f), Colour=col}
                }
                .Concat (
                    Enumerable.Range(1, vertices)
                    .Select(vertex => 2*Math.PI*vertex/vertices)
                    .Select(angleRadians => new Vertex4Plain{Position=new Vector4((float)(radius*Math.Cos(angleRadians)), (float)(radius*Math.Sin(angleRadians)), 0.0f, 1.0f), Colour=col})
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
