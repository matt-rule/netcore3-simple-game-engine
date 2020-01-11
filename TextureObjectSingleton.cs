using System.Collections.Generic;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public static class TextureObjectSingleton
    {
        public static List<TextureObject> textureObjects = new List<TextureObject>();

        public static TextureObject GetByName(string name)
        {
            return textureObjects.First(x => x.Name == name);
        }
    }
}