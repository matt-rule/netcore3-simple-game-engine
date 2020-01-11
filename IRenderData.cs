using OpenTK;
using OpenTK.Graphics;
using System;

namespace netcore3_simple_game_engine
{

    /// <summary>
    /// This object holds vertex data and - if necessary - the texture name to render.
    /// The Render function renders this object.
    /// This is handled with an interface so that an object
    /// can contain different types of rendering data
    /// (eg. textures vs colours) without being different types.
    /// 
    /// TODO: Consider replacing the use of interfaces using the
    /// OneOf library which supports sum types in dotnet core.
    /// </summary>
    public interface IRenderData: IDisposable
    {
        void Render();
    }
}