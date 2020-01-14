using OpenTK;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    public class Entity
    {
        public string Name;
        public IRenderData RenderData;
        public double RotationAngle;
        public double PaddleInitialY;
        
        public Entity(string name, IRenderData renderData, double rotationAngle, double paddleInitialY)
        {
            Name = name;
            RenderData = renderData;
            RotationAngle = rotationAngle;
            PaddleInitialY = paddleInitialY;
        }
    }
}
