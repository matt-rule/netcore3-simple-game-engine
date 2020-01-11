using OpenTK;
using OpenTK.Graphics.OpenGL4;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public Matrix4 ModelMatrix;
        public IRenderData RenderData;
        
        public Entity(string name, IRenderData renderData, Matrix4 modelMatrix)
        {
            Name = name;
            ModelMatrix = modelMatrix;
            RenderData = renderData;
        }
    }
}
