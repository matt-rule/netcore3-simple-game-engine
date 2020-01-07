namespace netcore3_simple_game_engine
{
    public class NonTexVertexData: IVertexData
    {
        public Vertex3d[] Vertices;

        public NonTexVertexData(Vertex3d[] vertices)
        {
            Vertices = vertices;
        }

        public void Initialise() {}

        public void Render()
        {

        }
    }
}