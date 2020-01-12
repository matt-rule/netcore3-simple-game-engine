using OpenTK;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public Matrix4 ModelMatrix;
        public IRenderData RenderData;
        public Vector2d? Velocity;
        
        public Entity(string name, Matrix4 modelMatrix, IRenderData renderData, Vector2d? velocity)
        {
            Name = name;
            ModelMatrix = modelMatrix;
            RenderData = renderData;
            Velocity = velocity;
        }
    }
}
