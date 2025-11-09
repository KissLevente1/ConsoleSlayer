using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using WMPLib;   

namespace ConsoleSlayer
{
    enum SoundEffectType {Door, AmmoPickup, Pain,BFGPickUp, HealthPickup, Death, Shoot, ShootBFG}

    internal class Game
    {
        Player player;
        bool isGameRunning;
        List<GameItem> items;
        List<Demon> demons;
        ConsoleRenderer consoleRenderer;
        GameLogic gameLogic;
        Stopwatch stopwatchLogic;
        Stopwatch stopwatchRenderer;
        WindowsMediaPlayer musicPlayer;
        WindowsMediaPlayer sfxPlayer;

        public bool IsGameRunning { get => isGameRunning; set => isGameRunning = value; }
        internal Player Player { get => player; set => player = value; }
        internal List<GameItem> Items { get => items; set => items = value; }
        internal List<Demon> Demons { get => demons; set => demons = value; }

        public Game()
        {
            isGameRunning = true;
            items = [];
            demons = [];
            consoleRenderer = new ConsoleRenderer(this);
            gameLogic = new GameLogic(this);
            stopwatchLogic = new Stopwatch();
            stopwatchRenderer = new Stopwatch();
            musicPlayer = new WindowsMediaPlayer();
            sfxPlayer = new WindowsMediaPlayer();
            
        }

        public void PlaySoundEffect(SoundEffectType soundEffectType)
        {
            switch (soundEffectType)
            {
                case SoundEffectType.Door:
                    sfxPlayer.URL = "Audio/door.mp3";
                    break;
                case SoundEffectType.Shoot:
                    sfxPlayer.URL = "Audio/shoot.wav";
                    break;
                case SoundEffectType.BFGPickUp:
                    sfxPlayer.URL = "Audio/pickup_bfgcell.wav";
                    break;
                case SoundEffectType.AmmoPickup:
                    sfxPlayer.URL ="Audio/pickup_ammo.wav";
                    break;
                case SoundEffectType.HealthPickup:
                    sfxPlayer.URL = "Audio/pickup_health.wav";
                    break;
                case SoundEffectType.ShootBFG:
                    sfxPlayer.URL = "Audio/shoot_bfg.wav";
                    break;
                case SoundEffectType.Death:
                    sfxPlayer.URL = "Audio/player_death.wav";
                    break;
            }
            if (sfxPlayer.playState != WMPPlayState.wmppsPlaying)
            {
                sfxPlayer.controls.play();
            }
            sfxPlayer.controls.play();

        }
        public void PlayMusic(string url)
        {
            musicPlayer.URL = url;
            musicPlayer.controls.play();
        }

        public void LoadMapFromPlainText(string filePath)
        {
            char[] items = new char[] {'D', 'E', 'T', 'W', 'A', 'B', 'M'};
            char[] demons = new char[] {'i', 'm', 'z'};
            using (StreamReader sr = new StreamReader(filePath))
            {   
                var dimensions = sr.ReadLine().Split(',');
                int x = int.Parse(dimensions[0]);
                int y = int.Parse(dimensions[1]);
                int a = 0;
                int b = 0;
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    foreach (char c in s)
                    {
                        if (items.Contains(c))
                        {
                            GameItem item = new GameItem(a,b, ItemType.BFGCell);
                            switch (c)
                            {
                                case 'D':
                                    item.Type = ItemType.Door;
                                    break;
                                case 'E':
                                    item.Type = ItemType.LevelExit;
                                    break;
                                case 'T':
                                    item.Type = ItemType.ToxicWaste;
                                    break;
                                case 'W':
                                    item.Type= ItemType.Wall;
                                    break;
                                case 'A':
                                    item.Type = ItemType.Ammo;
                                    break;
                                case 'B':
                                    item.Type = ItemType.BFGCell;
                                    break;
                                case 'M':
                                    item.Type = ItemType.Medikit;
                                    break;

                            }
                            item.SetInitialProperties();
                            this.items.Add(item);
                        }
                        else if(demons.Contains(c))
                        {
                            Demon demon = new Demon(a,b, DemonType.Imp);
                            switch (c)
                            {
                                case 'm':
                                    demon.Type = DemonType.Mancubus;
                                    break;
                                case 'z':
                                    demon.Type = DemonType.Zombieman;
                                    break;
                            }
                            this.demons.Add(demon);
                        }
                        else if(c == 'p')
                        {
                            Player player = new Player(a, b);
                            this.player = player;
                        }
                        b++;
                        
                    }
                    a++;
                    b = 0;
                }
            }
        }
        private void UserAction()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo pressed = Console.ReadKey(true); switch (pressed.Key)
                {
                    case ConsoleKey.Escape:
                        isGameRunning = false;
                        break;
                    case ConsoleKey.UpArrow:
                        gameLogic.Move(player, Position.Add(player.Position, new Position(0,-1)));
                        break;
                    case ConsoleKey.DownArrow:
                        gameLogic.Move(player, Position.Add(player.Position, new Position(0,1)));
                        break;
                    case ConsoleKey.RightArrow:
                        gameLogic.Move(player, Position.Add(player.Position, new Position(1,0)));
                        break;
                    case ConsoleKey.LeftArrow:
                        gameLogic.Move(player, Position.Add(player.Position, new Position(-1,0)));
                        break;
                    case ConsoleKey.D:
                        gameLogic.PlayerDirectInteractionLogic();
                        break;
                    case ConsoleKey.A:
                        gameLogic.PlayerAttackLogic(player);
                        break;
                    case ConsoleKey.S:
                        gameLogic.PlayerBFGAttackLogic();
                        break;
                }
            }
            gameLogic.PlayerIndirectInteractionLogic();
        }

        public void Run()
        {
            stopwatchLogic.Start();
            stopwatchRenderer.Start();
            PlayMusic("Audio/Demon's Dance.mp3");
            while(IsGameRunning && player.Alive)    
            {
                if (stopwatchLogic.ElapsedMilliseconds > 500)   
                {
                    gameLogic.UpdateGameState(stopwatchLogic.ElapsedMilliseconds);
                    stopwatchLogic.Restart();
                }
                if (stopwatchRenderer.ElapsedMilliseconds > 25) 
                { 
                    consoleRenderer.RenderGame();
                    stopwatchRenderer.Restart();
                }
                UserAction();
                Thread.Sleep(25);
            }

            consoleRenderer.RenderGame();
            if (player.Alive)
            {
                PlayMusic("Audio/Code and Dreams.mp3");
            }
            else
            {
                PlaySoundEffect(SoundEffectType.Death);
            }
            Console.ReadLine();
        }
    }
}
