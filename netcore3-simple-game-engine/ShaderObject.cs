using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.IO;

namespace netcore3_simple_game_engine
{
    public class ShaderObject : IDisposable
    {
        // ID of OpenGL vertex shader.
        public int VertexShaderId = -1;

        // ID of OpenGL fragment shader.
        public int FragmentShaderId = -1;

        // ID of shader program.
        public int ProgramId = -1;

        public int MatrixShaderLocation;


        // Name used to identify this shader program (not a filename).
        public string Name;
        
        private static int CompileShader(ShaderType type, String path)
        {
            var shader = GL.CreateShader(type);
            var src = File.ReadAllText(path);
            GL.ShaderSource(shader, src);
            GL.CompileShader(shader);
            var info = GL.GetShaderInfoLog(shader);
            if (!String.IsNullOrWhiteSpace(info))
                throw new Exception($"CompileShader {type} had errors: {info}");
            return shader;
        }

        public ShaderObject(string name, string vertexShaderFileName, string fragmentShaderFileName)
        {
            Name = name;
            
            try
            {
                ProgramId = GL.CreateProgram();
                VertexShaderId = CompileShader(ShaderType.VertexShader, vertexShaderFileName);
                FragmentShaderId = CompileShader(ShaderType.FragmentShader, fragmentShaderFileName);

                GL.AttachShader(ProgramId, VertexShaderId);
                GL.AttachShader(ProgramId, FragmentShaderId);
                GL.LinkProgram(ProgramId);
                String debugLog = GL.GetProgramInfoLog(ProgramId);
                if (!String.IsNullOrEmpty(debugLog))
                    Debug.WriteLine("Error: " + debugLog);

                MatrixShaderLocation = GL.GetUniformLocation(ProgramId, "mvp");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public void Dispose()
        {
            GL.DetachShader(ProgramId, VertexShaderId);
            GL.DeleteShader(VertexShaderId);

            GL.DetachShader(ProgramId, FragmentShaderId);
            GL.DeleteShader(FragmentShaderId);

            GL.DeleteProgram(ProgramId);
        }
    }
}
