using OpenTK;

using netcore3_simple_game_engine;

namespace netcore3_simple_game_engine
{
    /// <summary>
    /// Determines the sound that plays.
    /// </summary>
    public enum CollisionTypeEnum{Hard, Soft}

    public class Entity
    {
        public string Name;
        public IRenderData RenderData;
        public double RotationAngle;
        public double PaddleInitialY;
        public CollisionTypeEnum CollisionType;
        
        public Entity(string name, IRenderData renderData, double rotationAngle, double paddleInitialY, CollisionTypeEnum collisionType)
        {
            Name = name;
            RenderData = renderData;
            RotationAngle = rotationAngle;
            PaddleInitialY = paddleInitialY;
            CollisionType = collisionType;
        }
    }
}
