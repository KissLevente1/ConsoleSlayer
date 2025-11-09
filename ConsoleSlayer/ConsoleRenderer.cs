using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class ConsoleRenderer
    {
        Game game;
        public ConsoleRenderer(Game game)
        {
            this.game = game;
        }

        private void RenderSingleSprite(Position pos, ConsoleSprite consoleSprite)
        {
            if ((pos.X >= 0 && pos.X <= Console.WindowWidth) && (pos.Y >= 0 && pos.Y <= Console.WindowHeight))
            {
                Console.SetCursorPosition(pos.X, pos.Y);
                Console.BackgroundColor = consoleSprite.Background;
                Console.ForegroundColor = consoleSprite.Foreground;
                Console.WriteLine(consoleSprite.Glyph);
            }
        }

        private void RenderHUD()
        {
            string hud = $"HP:{game.Player.Health}  Ammo:{game.Player.Ammo}  Cells:{game.Player.BfgCells}  Combat Points:{game.Player.CombatPoints}";
            Console.SetCursorPosition(0, Console.WindowHeight-1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(hud);
        }

        public void RenderGame()
        {
            Console.CursorVisible = false;
            Console.ResetColor(); Console.Clear();
            foreach (GameItem item in game.Items)
            {
                double distance = Position.Distance(game.Player.Position, item.Position);
                if (distance<=game.Player.SightRange)
                {
                    RenderSingleSprite(item.Position, item.Sprite);
                }
            }
            foreach (Demon demon in game.Demons)
            {
                RenderSingleSprite(demon.Position, demon.Sprite);
            }
            RenderSingleSprite(game.Player.Position, game.Player.Sprite);
            RenderHUD();
        }
    }
}
