
namespace netcore3_simple_game_engine
{
    public interface IScene
    {
        // Usage: Call after OpenGL has been initialised.
        void Initialise();
        void Update(double elapsedTime);
    }
}