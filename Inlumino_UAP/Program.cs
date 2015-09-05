using System;

namespace Inlumino_UAP
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry Vector2 for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
#endif
}

