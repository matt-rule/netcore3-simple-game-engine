using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public static class ShaderObjectSingleton
    {
        public static List<ShaderObject> shaderObjects = new List<ShaderObject>();

        public static ShaderObject GetByName(string name)
        {
            return shaderObjects.First(x => x.Name == name);
        }
    }
};
