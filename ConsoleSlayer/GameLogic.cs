using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSlayer
{
    internal class GameLogic
    {
        Game game;
        public GameLogic(Game game)
        {
            this.game = game;
        }

        private void CleanUpDemons()
        {
            List<Demon> deadDemons = [];
            deadDemons = game.Demons.Where(d => !d.Alive).ToList();
            foreach (Demon demon in deadDemons)
            {
                if (game.Demons.Contains(demon))
                {
                    game.Demons.Remove(demon);
                }
            }
        }

        private void CleanUpGameItems()
        {
            List<GameItem> notAvalibleItems = [];
            notAvalibleItems = game.Items.Where(i => !i.Avalible).ToList();

            foreach (GameItem item in notAvalibleItems)
            {
                if (game.Items.Contains(item))
                {
                    game.Items.Remove(item);
                }
            }
        }

        private void DemonMoveLogic(Demon demon, long t)
        {
            Random rnd = new Random();
            int x = demon.Position.X + rnd.Next(-1, 2);
            int y = demon.Position.Y + rnd.Next(-1, 2);
            Position position = new Position(x, y);
            Move(demon, position, t);
        }

        private void DemonAttackLogic(Demon demon)
        {
            double distance = Position.Distance(game.Player.Position, demon.Position);
            Random rnd = new Random();
            int randomValue = 0;
            switch (demon.Type)
            {
                case DemonType.Zombieman:
                    randomValue = rnd.Next(3, 16);
                    break;
                case DemonType.Imp:
                    randomValue = rnd.Next(3, 25);
                    break;
                case DemonType.Mancubus:
                    randomValue = rnd.Next(8, 65);
                    break;
                default:
                    break;
            }
            int n = GetDemonsWithinDistance(demon.Position, distance).Count;
            double damage = (2 * randomValue) / (n * (1 + distance));
            if (damage!=0)
            {
                game.Player.TakeDamage((int)damage);
                game.PlaySoundEffect(SoundEffectType.Pain);
            }
        }

        private double GetTotalFillingRatio(Position pos)
        {
            return GetGameItemsWithinDistance(pos, 0).Sum(i => i.FillingRatio) + GetDemonsWithinDistance(pos, 0).Sum(d => d.FillingRatio);
        }

        private List<Demon> GetDemonsWithinDistance(Position pos, double distance)
        {
            return game.Demons.Where(d => Position.Distance(d.Position, pos) <= distance).ToList();
        }

        private List<GameItem> GetGameItemsWithinDistance(Position pos, double distance)
        {
            return game.Items.Where(i => Position.Distance(i.Position, pos) <= distance).ToList();
        }

        public void Move(Player player, Position pos)
        {
            double fillingRatioSum = player.FillingRatio + GetTotalFillingRatio(pos);
            if (fillingRatioSum <= 1)
            {
                player.Position = pos;
            }
        }
        private void Move(Demon demon, Position pos, long t)
        {
            float chance = ((float)demon.Speed / 100f) * ((float)t / 1000f);
            double fillingRatioSum = demon.FillingRatio + GetTotalFillingRatio(pos);
            Random rnd = new Random();
            float x = (float)rnd.NextDouble();
            if (fillingRatioSum <= 1 && x<chance)
            {   
                demon.Position = pos;
            }

        }

        private void UpdateDemons(long t)
        {
            foreach (Demon demon in game.Demons)
            {
                demon.UpdateState(game.Player);
                if (demon.State == DemonStateType.Move)
                {
                    DemonMoveLogic(demon, t);
                }
                else if (demon.State == DemonStateType.Attack)
                {
                    DemonAttackLogic(demon);
                }

                DemonIndirectInteractionLogic(demon);
            }
        }

        public void PlayerBFGAttackLogic()
        {
            if (game.Player.BfgCells!=0)
            {
                game.Player.ShootBFG();
                game.PlaySoundEffect(SoundEffectType.ShootBFG);
                List<Demon> demonsWithinSigthRange = GetDemonsWithinDistance(game.Player.Position, game.Player.SightRange);
                foreach (Demon demon in demonsWithinSigthRange)
                {
                    Random rnd = new Random();
                    demon.TakeDamage(rnd.Next(100, 801));
                    if (!demon.Alive)
                    {
                        game.Player.AddCombatPoints(1);
                    }
                }
            }
        }

        public void PlayerAttackLogic(Player player)
        {
            if (player.Ammo != 0)
            {
                player.Shoot();
                game.PlaySoundEffect(SoundEffectType.Shoot);
                List<Demon> demonsWithinDistance = GetDemonsWithinDistance(player.Position, player.SightRange);
                int n = demonsWithinDistance.Count;
                foreach (Demon demon in demonsWithinDistance)
                {
                    Random rnd = new Random();
                    double distance = Position.Distance(player.Position, demon.Position);

                    double damage = (2 * rnd.Next(36, 106)) / (n * (1 + distance));
                    demon.TakeDamage((int)damage);
                    if (!demon.Alive)
                    {
                        int combatPointsAfterKill = 0;
                        switch (demon.Type)
                        {
                            case DemonType.Zombieman:
                                combatPointsAfterKill = 1;
                                break;
                            case DemonType.Imp:
                                combatPointsAfterKill = 3;
                                break;
                            case DemonType.Mancubus:
                                combatPointsAfterKill = 10;
                                break;
                        }
                        player.AddCombatPoints(combatPointsAfterKill);
                    }
                }

            }
        }

        public void PlayerIndirectInteractionLogic()
        {
            List<GameItem> itemsWithinDistance = GetGameItemsWithinDistance(game.Player.Position, 0);
            foreach (GameItem item in itemsWithinDistance)
            {
                if (item.Type == ItemType.Ammo || item.Type == ItemType.ToxicWaste || item.Type == ItemType.Medikit || item.Type ==ItemType.BFGCell)
                {
                    if (item.Avalible)
                    {
                        item.Interact();
                        switch (item.Type)
                        {
                            case ItemType.Ammo:
                                if (game.Player.Ammo < game.Player.MaxAmmo)
                                {
                                    game.Player.PickUpAmmo(5);
                                    game.PlaySoundEffect(SoundEffectType.AmmoPickup);
                                }
                                break;
                            case ItemType.Medikit:
                                if (game.Player.Health < game.Player.MaxHealth)
                                {
                                    game.Player.PickUpHealth(25);
                                    game.PlaySoundEffect(SoundEffectType.HealthPickup);
                                }
                                break;
                            case ItemType.ToxicWaste:
                                game.Player.TakeDamage(5);
                                game.PlaySoundEffect(SoundEffectType.Pain);
                                break;
                            case ItemType.BFGCell:
                                game.Player.PickUpBFGCell();
                                game.PlaySoundEffect(SoundEffectType.BFGPickUp);
                                break;
                        }
                    }
                    
                }
            }
        }

        private void DemonIndirectInteractionLogic(Demon demon)
        {
            List<GameItem> itemsWithinDistance = GetGameItemsWithinDistance(demon.Position, 0);
            foreach (GameItem item in itemsWithinDistance)
            {
                if (item.Type ==ItemType.ToxicWaste)
                {
                    demon.TakeDamage(5);
                }
            }
        }


        public void PlayerDirectInteractionLogic()
        {
            List<GameItem> itemsWithinDistance = GetGameItemsWithinDistance(game.Player.Position, 1);
            foreach (GameItem item in itemsWithinDistance)
            {
                if (item.Type == ItemType.LevelExit || item.Type == ItemType.Door)
                {
                    item.Interact();
                    if (item.Type == ItemType.LevelExit)
                    {
                        game.IsGameRunning = false;
                    }
                    else
                    {
                        game.PlaySoundEffect(SoundEffectType.Door);
                    }

                }
            }
        }

        public void UpdateGameState(long t)
        {
            UpdateDemons(t);
            CleanUpGameItems();
            CleanUpDemons();
        }


    }
}
