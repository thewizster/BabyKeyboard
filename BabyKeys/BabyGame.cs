using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BabyKeys
{
    public class BabyGame : GameWindow
    {
        private float _red = 0.0f;
        private float _green = 0.0f;
        private float _blue = 0.0f;
        private double _timeSinceLastChange = 0.0;
        private readonly double _colorChangeInterval = 2.0; // Change color every 2 second

        public BabyGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(_red, _green, _blue, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Update the time since the last color change
            _timeSinceLastChange += args.Time;

            // Check if any key is pressed
            if (KeyboardState.IsAnyKeyDown)
            {
                // Change colors only if the interval has elapsed
                if (_timeSinceLastChange >= _colorChangeInterval)
                {
                    Random rand = new Random();
                    _red = (float)rand.NextDouble();
                    _green = (float)rand.NextDouble();
                    _blue = (float)rand.NextDouble();

                    // Update the clear color
                    GL.ClearColor(_red, _green, _blue, 1.0f);

                    // Reset the timer
                    _timeSinceLastChange = 0.0;
                }
            }

            if (KeyboardState.IsKeyDown(Keys.Escape) && KeyboardState.IsKeyDown(Keys.X))
            {
                // Pressing Escape and X at the same time will close the window
                Close();
            }
        }
    }
}
