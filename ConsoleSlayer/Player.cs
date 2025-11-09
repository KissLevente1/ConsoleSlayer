using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class Player
    {
        Position position;
        ConsoleSprite sprite;
        double fillingRatio;
        int ammo;
        int health;
        bool alive;
        int combatPoints;
        int sightRange;
        int maxHealth;
        int maxAmmo;
        int bfgCells;
        int maxBFGCells = 3;
        public double MaxHealth 
        {
            get { return maxHealth; }
            set { maxHealth = 100 + (combatPoints / 10); }
        }

        public double MaxAmmo
        {
            get { return maxAmmo; }
            set { maxAmmo = 10 + (combatPoints / 50); }
        }
        internal Position Position { get => position; set => position = value; }
        internal ConsoleSprite Sprite { get => sprite; set => sprite = value; }
        public double FillingRatio { get => fillingRatio; set => fillingRatio = value; }
        public int Ammo { get => ammo;
            set
            {
                if (value<=MaxAmmo)
                {
                    ammo = value;
                }
            }
        }
        public int Health { get => health;
            set 
            {
                if (value<=MaxHealth)
                {
                    health = value;
                }
            } 
        }
        public bool Alive { get => alive; set => alive = value; }
        public int CombatPoints { get => combatPoints; set => combatPoints = value; }
        public int SightRange { get => sightRange; set => sightRange = value; }
        public int BfgCells { get => bfgCells; set => bfgCells = value; }
        public int MaxBFGCells { get => maxBFGCells; set => maxBFGCells = value; }

        public Player(int x, int y)
        {
            position = new Position(x, y);
            sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Green, '0');
            fillingRatio = 0.4;
            health = 100;
            sightRange = 8;
            alive = true;
            ammo = 30;
            maxAmmo = 30;
            maxHealth = 100;    
        }

        public void PickUpAmmo(int amount)
        {
            if (ammo+amount<=MaxAmmo)
            {
                ammo += amount;
            }
        }

        public void PickUpHealth(int amount)
        {
            if (health+amount<=MaxHealth)
            {
                health += amount;
            }
        }

        public void PickUpBFGCell()
        {
            if (bfgCells+1<= maxBFGCells)
            {
                bfgCells++;
            }
        }
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                alive = false;
            }
        }

        public void AddCombatPoints(int points)
        {
            combatPoints += points;
        }

        public void Shoot()
        {
            if (ammo!=0) 
            {
                ammo--;
            } 
        }

        public void ShootBFG()
        {
            if (bfgCells!=0)
            {
                bfgCells--;
            }
        }


    }
}
