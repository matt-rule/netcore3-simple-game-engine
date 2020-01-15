using System.Collections.Generic;
using System.Linq;

namespace netcore3_simple_game_engine
{
    public static class TextureObjectSingleton
    {
        public static List<TextureObject> textureObjects = new List<TextureObject>();
        public static List<SpriteTextureObject> spriteTextureObjects = new List<SpriteTextureObject>();

        public static TextureObject GetSingleTextureByName(string name)
        {
            return textureObjects.First(x => x.Name == name);
        }

        public static SpriteTextureObject GetSpriteTextureByName(string name)
        {
            return spriteTextureObjects.First(x => x.Name == name);
        }
    }
}