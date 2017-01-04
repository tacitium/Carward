using System;

namespace Carward
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Carward game = new Carward())
            {
                game.Run();
            }
        }
    }
#endif
}

