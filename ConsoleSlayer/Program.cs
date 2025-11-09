using System.Collections.Concurrent;

namespace ConsoleSlayer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.LoadMapFromPlainText(@"..\..\..\src\data.txt");

            game.Run();
        }
    }
}
