using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class ConsoleSprite
    {
        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }
        public char Glyph { get; set; }

        public ConsoleSprite(ConsoleColor backGround, ConsoleColor foreGround, char glyph)
        {
            this.Background = backGround;
            this.Foreground= foreGround;   
            this.Glyph = glyph;
        }
    }
}
