using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using color = System.Drawing.Color;
using SharpDX;

namespace AJS.Utility.Ganksystem
{
    class GankManager
    {
        public static Menu GankMenu;

        public static Vector2 Botlane, Toplane, Midlane;

        public static AIHeroClient ADC { get; private set; }
        public static AIHeroClient Support { get; private set; }
        public static AIHeroClient Top { get; private set; }
        public static AIHeroClient Mid { get; private set; }
        public static AIHeroClient Jungle { get; private set; }

        public static bool TopFlash, MidFlash, BotFlash, TopWard, MidWard, BotWard;

        public static Spell.Active Topflash, Midflash, Adcflash;

        public static void OnLoad()
        {
            Drawing.OnDraw += OnDraw2;
            Menu();
        }

        public static void Menu()
        {
            var Mainmenu = Tracker.Tracker.Menu;
            GankMenu = Mainmenu.AddSubMenu("GankManager");

            GankMenu.AddGroupLabel("MenuPosition");
            GankMenu.Add("posX", new Slider("Adjust - X", 50, 0, 100));
            GankMenu.Add("posY", new Slider("Adjust - Y", 50, 0, 100));
            GankMenu.AddGroupLabel("Features");
            GankMenu.Add("activ", new CheckBox("Activate GankManager"));
            GankMenu.Add("reset", new CheckBox("Reset Roles", false));
            GankMenu.Add("names", new CheckBox("Show Summoner Names"));
            GankMenu.Add("life", new CheckBox("Life Check"));
            GankMenu.Add("flash", new CheckBox("Flash Check"));
            GankMenu.Add("ward", new CheckBox("Ward Check"));
            GankMenu.Add("suggestion", new CheckBox("Give Gank Suggestions"));

            Botlane = new Vector2(12567, 2572);
            Toplane = new Vector2(2287, 12581);
            Midlane = new Vector2(7503, 7402);
        }

        private static void OnDraw(EventArgs args)
        {
            float gapPos = 0;

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (GankMenu["activ"].Cast<CheckBox>().CurrentValue)
                {
                    gapPos += 15;
                    Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.White, enemy.ChampionName);

                    if (GankMenu["life"].Cast<CheckBox>().CurrentValue)
                    {
                        if (enemy.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 100, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.GreenYellow, "L " + (int)enemy.HealthPercent + "%");
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 100, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Yellow, "L Rip");
                    }

                    if (GankMenu["flash"].Cast<CheckBox>().CurrentValue)
                    {
                        var FlashSlot = enemy.Spellbook.Spells[4];

                        if (FlashSlot.Name != "summonerflash")
                            FlashSlot = enemy.Spellbook.Spells[5];

                        if (FlashSlot.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlot.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.GreenYellow, "F rdy");
                            else
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                }
            }
        }

        private static void OnDraw2(EventArgs args)
        {
            float gapPos = 0;

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.Position.Distance(Toplane) < 4000 && Top == null)
                {
                    Top = enemy;
                }

                if (enemy.Position.Distance(Midlane) < 4000 && Mid == null)
                {
                    Mid = enemy;
                }

                if (enemy.Position.Distance(Botlane) < 4000 && (enemy.GetSpellSlotFromName("summonerheal") == SpellSlot.Summoner1 || enemy.GetSpellSlotFromName("summonerheal") == SpellSlot.Summoner2) && ADC == null)
                {
                    ADC = enemy;
                }

                if (enemy.Position.Distance(Botlane) < 4000 && enemy.GetSpellSlotFromName("summonerheal") != SpellSlot.Summoner1 && enemy.GetSpellSlotFromName("summonerheal") != SpellSlot.Summoner2 && Support == null)
                {
                    Support = enemy;
                }

                if ((enemy.GetSpellSlotFromName("summonersmite") == SpellSlot.Summoner1 || enemy.GetSpellSlotFromName("summonersmite") == SpellSlot.Summoner2) && Jungle == null)
                {
                    Jungle = enemy;
                }

                if (GankMenu["reset"].Cast<CheckBox>().CurrentValue)
                {
                    Top = null;
                    Mid = null;
                    ADC = null;
                    Support = null;
                    Jungle = null;
                }
            }

            if (GankMenu["activ"].Cast<CheckBox>().CurrentValue)
            {
                gapPos += 15;
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos - 15, color.White, "GankManager");
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.White, "Top");
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.White, "Mid");
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.White, "Jungle");
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.White, "Bot");
                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.White, "Support");

                //if (Top == null) return;
                //if (Mid == null) return;
                //if (ADC == null) return;
                //if (Jungle == null) return;
                //if (Support == null) return;

                if (GankMenu["names"].Cast<CheckBox>().CurrentValue)
                {
                    if (Top != null) Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 60, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.White, "(" + Top.BaseSkinName + ")");
                    if (Mid != null) Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 60, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.White, "(" + Mid.BaseSkinName + ")");
                    if (Jungle != null) Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 60, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.White, "(" + Jungle.BaseSkinName + ")");
                    if (ADC != null) Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 60, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.White, "(" + ADC.BaseSkinName + ")");
                    if (Support != null) Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 60, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.White, "(" + Support.BaseSkinName + ")");
                }

                if (GankMenu["life"].Cast<CheckBox>().CurrentValue)
                {
                    if (Top != null)
                    {
                        if (Top.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, (Top.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Top.HealthPercent + "%");
                        else if (Top != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Red, "Dead");
                    }

                    if (Mid != null)
                    {
                        if (Mid.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, (Mid.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Mid.HealthPercent + "%");
                        else if (Mid != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Red, "Dead");
                    }
                    if (Jungle != null)
                    {
                        if (Jungle.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, (Jungle.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Jungle.HealthPercent + "%");
                        else if (Jungle != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.Red, "Dead");
                    }
                    if (ADC != null)
                    {
                        if (ADC.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, (ADC.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)ADC.HealthPercent + "%");
                        else if (ADC != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Red, "Dead");
                    }
                    if (Support != null)
                    {
                        if (Support.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, (Support.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Support.HealthPercent + "%");
                        else if (Support != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 120, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Red, "Dead");
                    }
                }

                if (GankMenu["flash"].Cast<CheckBox>().CurrentValue)
                {
                    if (Top != null)
                    {
                        var FlashSlotTOP = Top.Spellbook.Spells[4];

                        if (FlashSlotTOP.Name != "summonerflash")
                            FlashSlotTOP = Top.Spellbook.Spells[5];

                        if (FlashSlotTOP.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlotTOP.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                            {
                                TopFlash = true;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.GreenYellow, "F rdy");
                            }
                            else
                                TopFlash = false;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                    if (Mid != null)
                    {
                        var FlashSlotMID = Mid.Spellbook.Spells[4];

                        if (FlashSlotMID.Name != "summonerflash")
                            FlashSlotMID = Mid.Spellbook.Spells[5];

                        if (FlashSlotMID.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlotMID.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                            {
                                MidFlash = true;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.GreenYellow, "F rdy");
                            }
                            else
                                MidFlash = false;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                    if (Jungle != null)
                    {
                        var FlashSlotJungle = Jungle.Spellbook.Spells[4];

                        if (FlashSlotJungle.Name != "summonerflash")
                            FlashSlotJungle = Jungle.Spellbook.Spells[5];

                        if (FlashSlotJungle.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlotJungle.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.GreenYellow, "F rdy");
                            else
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                    if (ADC != null)
                    {
                        var FlashSlotADC = ADC.Spellbook.Spells[4];

                        if (FlashSlotADC.Name != "summonerflash")
                            FlashSlotADC = ADC.Spellbook.Spells[5];

                        if (FlashSlotADC.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlotADC.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                            {
                                BotFlash = true;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.GreenYellow, "F rdy");
                            }
                            else
                                BotFlash = false;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                    if (Support != null)
                    {
                        var FlashSlotSupp = Support.Spellbook.Spells[4];

                        if (FlashSlotSupp.Name != "summonerflash")
                            FlashSlotSupp = Support.Spellbook.Spells[5];

                        if (FlashSlotSupp.Name == "summonerflash")
                        {
                            var FlashTime = FlashSlotSupp.CooldownExpires - Game.Time;

                            if (FlashTime < 0)
                            {
                                BotFlash = true;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.GreenYellow, "F rdy");
                            }
                            else
                                BotFlash = false;
                                Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Yellow, "F " + (int)FlashTime);
                        }
                    }
                }

                if (GankMenu["ward"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var ward in WardTracker.WardTracker.Wards)
                    {
                        if (ward.Position.Distance(Toplane) < 4000)
                        {
                            TopWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Red, "Warded");
                        }
                        else if (ward == null)
                        {
                            TopWard = false;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Green, "Not Warded");
                        }
                        if (ward.Position.Distance(Midlane) < 4000)
                        {
                            MidWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Red, "Warded");
                        }
                        else if (ward == null)
                        {
                            MidWard = false;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Green, "Not Warded");
                        }
                        if (ward.Position.Distance(Botlane) < 4000)
                        {
                            BotWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Red, "Warded");
                        }
                        else if (ward == null)
                        {
                            BotWard = false;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Green, "Not Warded");
                        }
                        if (ward.Position.Distance(Botlane) < 4000)
                        {
                            BotWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Red, "Warded");
                        }
                        else if (ward == null)
                        {
                            BotWard = false;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 240, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Green, "Not Warded");
                        }
                    }
                }

                if (GankMenu["suggestion"].Cast<CheckBox>().CurrentValue)
                {
                    if (TopWard == true && TopFlash == true && BotWard == true && BotFlash == true && MidWard == true && MidFlash == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Red, "All warded as well as flash rdy!");
                    }
                    if (TopWard == true && MidWard == true && BotWard == true && TopFlash == false && BotFlash == true && MidFlash == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Top no flash!");
                    }
                    if (TopWard == true && MidWard == true && BotWard == true && MidFlash == false && BotFlash == true && TopFlash == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Mid no flash!");
                    }
                    if (TopWard == true && MidWard == true && BotWard == true && BotFlash == false && TopFlash == true && MidFlash == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Bot no flash!");
                    }
                    if (TopFlash == true && BotFlash == true && MidFlash == true && BotWard == false && TopWard == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Bot no ward!");
                    }
                    if (TopFlash == true && BotFlash == true && MidFlash == true && TopWard == false && BotWard == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Top no ward!");
                    }
                    if (TopFlash == true && BotFlash == true && MidFlash == true && MidWard == false && TopWard == true && BotWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Mid no ward!");
                    }
                    if (TopFlash == false && TopWard == false && BotWard == true && BotWard == true && MidFlash == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Top!!!");
                    }
                    if (BotFlash == false && BotWard == false && TopWard == true && TopWard == true && MidFlash == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == true && TopWard == true && BotFlash == true && BotWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == false && TopFlash == false && BotFlash == true && BotWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Top!!!");
                    }
                    if (MidFlash == false && MidWard == false && BotWard == false && BotFlash == false && TopFlash == true && TopWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Bot!!!");
                    }
                    if (BotWard == false && BotFlash == false && TopFlash == false && TopWard == false && MidFlash == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot/Top!!!");
                    }
                    if (BotWard == false && BotFlash == false && TopFlash == false && TopWard == false && MidFlash == false && MidWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "No wards or flashs rdy!!!");
                    }
                }
            }
        }
    }
}

  

