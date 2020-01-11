using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public struct ShaderProgram: IDisposable
    {
        public int ProgramId;
        public List<int> Shaders;
        public int MatrixShaderLocation;

        public void Dispose()
        {
            foreach (var shader in Shaders)
            {
                GL.DetachShader(ProgramId, shader);
                GL.DeleteShader(shader);
            }
            GL.DeleteProgram(ProgramId);
        }
    }

    public static class ShaderObjectSingleton
    {
        public static ShaderProgram MainShaderProgram;
    };
};
