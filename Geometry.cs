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
                Vertices = new Vertex3d[] {
                    new Vertex3d(new Vector4( -width/2, -height/2, 0.0f, 1.0f), col),
                    new Vertex3d(new Vector4(  width/2, -height/2, 0.0f, 1.0f), col),
                    new Vertex3d(new Vector4(  width/2,  height/2, 0.0f, 1.0f), col),
                    new Vertex3d(new Vector4( -width/2,  height/2, 0.0f, 1.0f), col),
                },
                Indices = new uint[] {
                    0, 1, 2, 0, 2, 3
                }
            };
        }

        public static BufferData Circle(int vertices, float radius, Color4 col)
        {
            return new BufferData {
                Vertices = new List<Vertex3d> {
                    new Vertex3d(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), col)
                }
                .Concat (
                    Enumerable.Range(0, vertices)
                    .Select(x => new Vertex3d(new Vector4((float)(radius*Math.Cos(x)), (float)(radius*Math.Sin(x)), 0.0f, 1.0f), col))
                )
                .ToArray(),
                Indices = Enumerable.Range(0, vertices)
                .SelectMany(x => new uint[]{0, (uint)x, (uint)((x + 1) < vertices ? (x + 1) : 1)})
                .ToArray()
            };
        }
    }
}
