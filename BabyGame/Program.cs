using System.Threading;
using System;

using var mutex = new Mutex(true, "BabyGameSingletonMutex", out bool isNewInstance);

if (!isNewInstance)
{
    Console.WriteLine("Another instance of the application is already running.");
    return;
}

using var game = new BabyGame.BabyGame();
game.Run();
