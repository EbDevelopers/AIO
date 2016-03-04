using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;

namespace PathTracker
{
    class PathTracker
    {
        public static void OnLoad()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }
        public static Vector3 WayPoint(AIHeroClient hero)
        {
            var i = 1;
            return hero.Path[i - 1];
            
        }
        public static float Eta(AIHeroClient hero)
        {
            var x1 = hero.Distance(WayPoint(hero));
            var x2 = x1 / hero.MoveSpeed;
            return x2;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Tracker.Tracker.lala["trackallyspath"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var ally in EntityManager.Heroes.Allies.Where(x => !x.IsMe && ObjectManager.Player.Distance(x.Position) < 1000))
                {
                    if (Tracker.Tracker.lala["eta"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(ally)).X + 20, (int)Drawing.WorldToScreen(WayPoint(ally)).Y + 20, System.Drawing.Color.Gold, "" + Eta(ally));
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(ally.Position), Drawing.WorldToScreen(WayPoint(ally)), 2, System.Drawing.Color.Gold);
                }
            }
            if (Tracker.Tracker.lala["trackenemypath"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => ObjectManager.Player.Distance(x.Position) < 1000))
                {
                    if (Tracker.Tracker.lala["eta"].Cast<CheckBox>().CurrentValue)
                    {
                        Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(enemy)).X + 20, (int)Drawing.WorldToScreen(WayPoint(enemy)).Y + 20, System.Drawing.Color.Gold, "" + Eta(enemy));
                    }
                    Drawing.DrawLine(Drawing.WorldToScreen(enemy.Position), Drawing.WorldToScreen(WayPoint(enemy)), 2, System.Drawing.Color.Gold);
                }
            }
            if (Tracker.Tracker.lala["trackmypath"].Cast<CheckBox>().CurrentValue)
            {
                if (Tracker.Tracker.lala["eta"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawText((int)Drawing.WorldToScreen(WayPoint(ObjectManager.Player)).X + 20, (int)Drawing.WorldToScreen(WayPoint(ObjectManager.Player)).Y + 20, System.Drawing.Color.Gold, "" + Eta(ObjectManager.Player));
                }
                Drawing.DrawLine(Drawing.WorldToScreen(ObjectManager.Player.Position), Drawing.WorldToScreen(WayPoint(ObjectManager.Player)), 2, System.Drawing.Color.Gold);
            }
        }
    }
}