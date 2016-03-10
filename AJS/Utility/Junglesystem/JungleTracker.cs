using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using Vector2 = SharpDX.Vector2;

namespace AJS.Utility.Junglesystem
{
    class JungleTracker
    {
        public static Menu _menu;

        #region Definitions

        /*
        camp.State == 0 Not Tracking
        camp.State == 1 Attacking
        camp.State == 2 Disengaged
        camp.State == 3 Tracking/Iddle
        camp.State == 4 Presumed Dead
        camp.State == 5 Guessed on fow with networkId
        camp.State == 6 Guessed maybe dead
        camp.State == 7 dead on timer to respawn
        */

        public static IEnumerable<Obj_AI_Minion> Jungle
        {
            get { return JungleList; }
        }

        private static readonly HashSet<Obj_AI_Minion> JungleList = new HashSet<Obj_AI_Minion>();

        private const float CheckInterval = 800f;
        private static readonly List<Camp> _camps = new List<Camp>();
        private static int _dragonStacks;
        private static float _lastCheck = Environment.TickCount;



        #endregion

        public static void Menu()
        {
            var Mainmenu = Tracker.Tracker.Menu;
            var Trackermenu = Tracker.Tracker.lala;
            Trackermenu.AddGroupLabel("JungleTracker");
            Trackermenu.Add("track2", new CheckBox("Track Map"));
            Trackermenu.Add("minitrack", new CheckBox("Track MiniMap"));

        }

        public static void OnLoad()
        {
            Menu();
            foreach (var camp in _camps)
            {
                Drawing.DrawCircle(camp.Position, 50, Color.AliceBlue);
            }
            Drawing.OnEndScene += Drawing_OnEndScene;
            GameObject.OnCreate += GameObjectOnCreate;
            GameObject.OnDelete += GameObjectOnDelete;
            Game.OnUpdate += OnGameUpdate;
        }

        public static void GameObjectOnCreate(GameObject sender, EventArgs args)
        {

            if (!sender.IsValid || sender.Type != GameObjectType.obj_AI_Minion ||
                sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            foreach (var camp in _camps)
            {
                var mob =
                    camp.Mobs.FirstOrDefault(m => m.Name.Contains(sender.Name));
                if (mob != null)
                {
                    mob.Dead = false;
                    camp.Dead = false;
                }
            }
        }
        

        public static void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid || sender.Type != GameObjectType.obj_AI_Minion ||
               sender.Team != GameObjectTeam.Neutral)
            {
                return;
            }

            foreach (var camp in _camps.ToArray())
            {
                var mob =
                    camp.Mobs.FirstOrDefault(m => m.Name.Contains(sender.Name));
                if (mob != null)
                {
                    if (mob.Name.Contains("Herald"))
                    {
                        if (Game.Time + camp.RespawnTime > 20 * 60 ||
                            Jungle.Any(j => j.CharData.BaseSkinName.Contains("Baron")))
                        {
                            _camps.Remove(camp);
                            continue;
                        }
                    }
                    mob.Dead = true;
                    camp.Dead = camp.Mobs.All(m => m.Dead);
                    if (camp.Dead)
                    {
                        camp.Dead = true;
                        camp.NextRespawnTime = (int)Game.Time + camp.RespawnTime - 3;
                    }
                }
            }
        }
    

        public static void OnGameUpdate(EventArgs args)
        {
            if (_lastCheck + CheckInterval > Environment.TickCount)
            {
                return;
            }

            _lastCheck = Environment.TickCount;

            var dragonStacks = 0;
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                var buff =
                    enemy.Buffs.FirstOrDefault(
                        b => b.Name.Equals("s5test_dragonslayerbuff", StringComparison.OrdinalIgnoreCase));
                if (buff != null)
                {
                    dragonStacks = buff.Count;
                }
            }

            if (dragonStacks > _dragonStacks || dragonStacks == 5)
            {
                var dCamp = _camps.FirstOrDefault(c => c.Mobs.Any(m => m.Name.Contains("Dragon")));
                if (dCamp != null && !dCamp.Dead)
                {
                    dCamp.Dead = true;
                    dCamp.NextRespawnTime = (int)Game.Time + dCamp.RespawnTime;
                }
            }

            _dragonStacks = dragonStacks;

            var bCamp = _camps.FirstOrDefault(c => c.Mobs.Any(m => m.Name.Contains("Baron")));
            if (bCamp != null && !bCamp.Dead)
            {
                var heroes = EntityManager.Heroes.Enemies.Where(e => e.IsVisible);
                foreach (var hero in heroes)
                {
                    var buff =
                        hero.Buffs.FirstOrDefault(
                            b => b.Name.Equals("exaltedwithbaronnashor", StringComparison.OrdinalIgnoreCase));
                    if (buff != null)
                    {
                        bCamp.Dead = true;
                        bCamp.NextRespawnTime = (int)buff.StartTime + bCamp.RespawnTime;
                    }
                }
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            var mapEnabled = Tracker.Tracker.lala["track2"].Cast<CheckBox>().CurrentValue;
            var minimapEnabled = Tracker.Tracker.lala["minitrack"].Cast<CheckBox>().CurrentValue;

            if (!mapEnabled && !minimapEnabled)
            {
                return;
            }

            foreach (var camp in _camps.Where(c => c.Dead))
            {
                if (camp.NextRespawnTime - Game.Time <= 0)
                {
                    camp.Dead = false;
                    continue;
                }

                if (mapEnabled && camp.Position.IsOnScreen())
                {
                    Drawing.DrawCircle(camp.Position, 50, Color.AliceBlue);
                    Drawing.DrawText(Drawing.WorldToScreen(camp.Position).X, Drawing.WorldToScreen(camp.Position).Y, Color.White, string.Format("{0}:{1:00}", (camp.NextRespawnTime - (int)Game.Time)));
                }
                if (minimapEnabled)
                {
                    Drawing.DrawText(camp.MinimapPosition.X, camp.MinimapPosition.Y, Color.White, string.Format("{0}:{1:00}", (camp.NextRespawnTime - (int)Game.Time)));
                }
            }
        }
    

        private class Camp : Jungle.Camp
        {
            public Camp(float spawnTime,
                float respawnTime,
                Vector3 position,
                List<Jungle.Mob> mobs,
                bool isBig,
                GameObjectTeam team,
                bool dead = false) : base(spawnTime, respawnTime, position, mobs, isBig, team)
            {
                Dead = dead;
                Mobs = mobs.Select(mob => new Mob(mob.Name)).ToList();
            }

            public new List<Mob> Mobs { get; private set; }
            public float NextRespawnTime { get; set; }
            public bool Dead { get; set; }
        }

        class Mob : Jungle.Mob
        {
            public Mob(string name, bool dead = false) : base(name)
            {
                Dead = dead;
            }

            public bool Dead { get; set; }
        }

    }
}