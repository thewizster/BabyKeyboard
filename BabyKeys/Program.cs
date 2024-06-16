using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace BabyKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameWindowSettings = GameWindowSettings.Default;
            var nativeWindowSettings = new NativeWindowSettings()
            {
                StartVisible = true,
                WindowState = WindowState.Normal,
                Title = "OpenTK Fullscreen App",
                NumberOfSamples = 4 // Anti-aliasing
            };

            // Get total screen size using the helper
            var (totalWidth, totalHeight, minX, minY) = ScreenHelper.GetTotalScreenSize();
            nativeWindowSettings.Size = new Vector2i(totalWidth, totalHeight);
            nativeWindowSettings.Location = new Vector2i(minX, minY);

            using (var game = new BabyGame(gameWindowSettings, nativeWindowSettings))
            {
                game.WindowBorder = WindowBorder.Hidden; // Hide window borders
                game.Location = new Vector2i(minX, minY); // Explicitly set the location
                game.Run();
            }
        }
    }
}
