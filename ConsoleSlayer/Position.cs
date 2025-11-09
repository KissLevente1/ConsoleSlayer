using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class Position
    {
        int x;
        int y;

        

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public static Position Add(Position p1, Position p2)
        {
            return new Position(p1.X + p2.x, p1.y + p2.y);
        }

        public static float Distance(Position p1, Position p2)
        {
            return MathF.Abs(p1.x - p2.x) + MathF.Abs(p1.y - p2.y);
        }
    }
}
