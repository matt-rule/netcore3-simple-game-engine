using OpenTK;
using OpenTK.Graphics;
using System;

namespace netcore3_simple_game_engine
{

    public delegate void ResizeDelegate(EventArgs e);

    public delegate void LoadDelegate(EventArgs e);

    public struct WindowOptions
    {
        public int Width;
        public int Height;
        public int GlVersionMajor;
        public int GlVersionMinor;
        public string Title;
        public string VertexShaderFileName;
        public string FragmentShaderFileName;
        public IScene Scene;

        public ResizeDelegate ResizeHandler;
        public LoadDelegate LoadHandler;
    }

    public class MainWindow : GameWindow
    {

        public ResizeDelegate ResizeHandler;
        public LoadDelegate LoadHandler;
        public IScene Scene;

        MainWindow(WindowOptions options)
            : base(options.Width,
                options.Height,
                GraphicsMode.Default,
                options.Title,
                GameWindowFlags.Default,
                DisplayDevice.Default,
                options.GlVersionMajor,
                options.GlVersionMinor,
                GraphicsContextFlags.ForwardCompatible)
        {
            if (
                options.ResizeHandler == null
                || options.LoadHandler == null
            )
            {
                throw new Exception("None of the event handlers should be null.");
            }

            if (options.Width <= 0 || options.Height <= 0)
            {
                throw new Exception("Both Width and Height should be more than 0.");
            }

            ResizeHandler = options.ResizeHandler;
            LoadHandler = options.LoadHandler;
            Scene = options.Scene;
        }

        protected override void OnResize(EventArgs e)
        {
            ResizeHandler(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadHandler(e);
            Scene.Initialise();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Scene.Update(e.Time);
        }
    }
}
