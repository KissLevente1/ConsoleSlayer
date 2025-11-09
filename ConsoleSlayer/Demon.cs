using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ConsoleSlayer
{
    internal class Demon
    {
        Position position;
        ConsoleSprite sprite;
        DemonType type;
        double fillingRatio;
        int health;
        bool alive;
        int sightRange;
        int attackRange;
        DemonStateType state;
        int speed;

        public double FillingRatio { get => fillingRatio; set => fillingRatio = value; }
        public int Health { get => health; set => health = value; }
        public bool Alive { get => alive; set => alive = value; }
        public int SightRange { get => sightRange; set => sightRange = value; }
        public int AttackRange { get => attackRange; set => attackRange = value; }

        internal Position Position { get => position; set => position = value; }
        internal ConsoleSprite Sprite { get => sprite; set => sprite = value; }
        internal DemonType Type { get => type; set => type = value; }
        internal DemonStateType State { get => state; set => state = value; }
        public int Speed { get => speed; set => speed = value; }

        private void SetInitialProperties()
        {
            switch (type)
            {
                case DemonType.Zombieman:
                    fillingRatio = 0.4;
                    health = 20;
                    sightRange = 3;
                    attackRange = 1;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.White, 'o');
                    speed = 70;
                    break;
                case DemonType.Imp:
                    fillingRatio = 0.4;
                    health = 60;
                    sightRange = 6;
                    attackRange = 3;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Red, 'o');
                    speed = 93;
                    break;
                case DemonType.Mancubus:
                    fillingRatio = 0.96;
                    speed = 70;
                    health = 600;
                    sightRange = 9;
                    attackRange = 6;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Magenta, 'O');
                    break;
            }
        }
        public Demon(int x, int y, DemonType demonType)
        {
            type = demonType;
            position = new Position(x, y);
            alive = true;
            state = DemonStateType.Idle;
            SetInitialProperties();
        }
        public void UpdateState(Player player)
        {
            double distance = Position.Distance(player.Position, position);
            if (distance <= attackRange)
            {
                state = DemonStateType.Attack;
            }
            else if (distance <= sightRange)
            {
                state = DemonStateType.Move;
            }
            else state = DemonStateType.Idle;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health<=0)
            {
                alive = false;
            }
        }
    }
    enum DemonType { Zombieman, Imp, Mancubus }
    enum DemonStateType { Idle, Move, Attack }
}
