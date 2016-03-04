using System;
using System.Linq;
using System.Xml.Schema;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace Tracker
{
    class Tracker
    {
        public static Menu Menu, lala;

        public static void OnLoad()
        {
            Menu = MainMenu.AddMenu("Utility", "utility");
            {
                lala = Menu.AddSubMenu("Tracker", "WardTracker");
                lala.AddGroupLabel("Ward Tracker");
                lala.Add("track", new CheckBox("Track enemy wards"));
                lala.AddGroupLabel("Spell Tracker");
                lala.Add("trackallyspells", new CheckBox("Track ally Spells", false));
                lala.Add("trackmyspells", new CheckBox("Track my Spells", false));
                lala.Add("trackenemyspells", new CheckBox("Track enemy Spells"));
                lala.AddGroupLabel("Path Tracker");
                lala.Add("trackallyspath", new CheckBox("Track ally Path", false));
                lala.Add("trackenemypath", new CheckBox("Track enemy Path"));
                lala.Add("trackmypath", new CheckBox("Track my Path", false));
                lala.Add("eta", new CheckBox("ETA"));
            }
           
                Drawing.OnDraw += OnDraw;

        }
        private static void OnDraw(EventArgs args)
        {
            if (lala["trackmyspells"].Cast<CheckBox>().CurrentValue)
            {
                SpellTracker.SpellTracker.PlayerTracker();
            }
            if (lala["trackallyspells"].Cast<CheckBox>().CurrentValue)
            {
                SpellTracker.SpellTracker.AllyTracker();
            }
            if (lala["trackenemyspells"].Cast<CheckBox>().CurrentValue)
            {
                SpellTracker.SpellTracker.EnemyTracker();
            }
        }
    }
}