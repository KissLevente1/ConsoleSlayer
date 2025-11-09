using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class GameItem
    {
        Position position;
        ConsoleSprite sprite;
        ItemType type;
        double fillingRatio;
        bool avalible;

        public double FillingRatio { get => fillingRatio; set => fillingRatio = value; }
        public bool Avalible { get => avalible; set => avalible = value; }
        internal Position Position { get => position; set => position = value; }
        internal ConsoleSprite Sprite { get => sprite; set => sprite = value; }
        internal ItemType Type { get => type; set => type = value; }

        public GameItem(int x, int y, ItemType itemType)
        {
            position = new Position(x, y);
            type = itemType;
            avalible = true;
            SetInitialProperties();
        }
        public void SetInitialProperties()
        {
            
            switch (type)
            {
                case ItemType.Ammo:
                    fillingRatio = 0;
                    sprite = new ConsoleSprite(ConsoleColor.Red, ConsoleColor.Yellow, 'A');
                    break;

                case ItemType.BFGCell:
                    fillingRatio = 0;
                    sprite = new ConsoleSprite(ConsoleColor.Green, ConsoleColor.White, 'B');
                    break;

                case ItemType.Door:
                    fillingRatio = 1;
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Yellow, '/');
                    break;

                case ItemType.LevelExit:
                    fillingRatio = 1;
                    sprite = new ConsoleSprite(ConsoleColor.Blue, ConsoleColor.Black, 'E');
                    break;
                case ItemType.Medikit:
                    fillingRatio = 0;
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Red, '+');
                    break;
                case ItemType.ToxicWaste:
                    fillingRatio = 0;
                    sprite = new ConsoleSprite(ConsoleColor.Green, ConsoleColor.Yellow, ':');
                    break;
                case ItemType.Wall:
                    fillingRatio = 1;
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Gray, ' ');
                    break;
                default:
                    break;
            }
        }
        public void Interact()
        {
            if (avalible)
            {
                if (type == ItemType.BFGCell || type == ItemType.Ammo || type == ItemType.Medikit)
                {
                    avalible = false;
                }
                else if (type == ItemType.Door)
                {
                    if (fillingRatio == 0)
                    {
                        fillingRatio = 1;
                        sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Yellow, '/');
                    }
                    else
                    {
                        fillingRatio = 0;
                        sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.DarkYellow, '/');
                    }
                }
            }
           
        }
    }

    enum ItemType { Ammo, BFGCell, Door, LevelExit, Medikit, ToxicWaste, Wall }
}
