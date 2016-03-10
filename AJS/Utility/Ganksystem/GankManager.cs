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

        private static readonly SpellSlot[] Summoners =
        {
            SpellSlot.Summoner1,
            SpellSlot.Summoner2,
        };

        public static string[] SummonersNames =
        {
            "summonerbarrier", "summonerboost", "summonerdot", "summonerexhaust",
            "summonerflash", "summonerhaste", "summonerheal", "summonerodinGarrison",
            "summonerteleport",
        };

        public static string GetSummonerName(Obj_AI_Base hero, SpellSlot slot)
        {
            return hero.Spellbook.GetSpell(slot).SData.Name;
        }

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

            Region.Initialize();
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
                if (Region.OnTopLane(enemy) && Top == null)
                {
                    Top = enemy;
                }

                if (Region.OnMidLane(enemy) && Mid == null)
                {
                    Mid = enemy;
                }

                if (Region.OnBotLane(enemy) && (enemy.GetSpellSlotFromName("summonerheal") == SpellSlot.Summoner1 || enemy.GetSpellSlotFromName("summonerheal") == SpellSlot.Summoner2) && ADC == null)
                {
                    ADC = enemy;
                }

                if (Region.OnBotLane(enemy) && enemy.GetSpellSlotFromName("summonerheal") != SpellSlot.Summoner1 && enemy.GetSpellSlotFromName("summonerheal") != SpellSlot.Summoner2 && Support == null)
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
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, (Top.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Top.HealthPercent + "%");
                        else if (Top != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Red, "Dead");
                    }

                    if (Mid != null)
                    {
                        if (Mid.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, (Mid.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Mid.HealthPercent + "%");
                        else if (Mid != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Red, "Dead");
                    }
                    if (Jungle != null)
                    {
                        if (Jungle.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, (Jungle.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Jungle.HealthPercent + "%");
                        else if (Jungle != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.Red, "Dead");
                    }
                    if (ADC != null)
                    {
                        if (ADC.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, (ADC.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)ADC.HealthPercent + "%");
                        else if (ADC != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Red, "Dead");
                    }
                    if (Support != null)
                    {
                        if (Support.HealthPercent > 0)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, (Support.HealthPercent > 50 ? color.GreenYellow : color.Yellow), "L " + (int)Support.HealthPercent + "%");
                        else if (Support != null)
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 160, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Red, "Dead");
                    }
                }

                if (GankMenu["flash"].Cast<CheckBox>().CurrentValue)
                {
                    if (Top != null)
                    {
                        var slot = Top.GetSpellSlotFromName("summonerflash");

                        var cooldown = Top.Spellbook.GetSpell(slot).CooldownExpires - Game.Time;
                        if (cooldown < 0)
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Red, "F rdy");
                            BotFlash = true;
                        }
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.GreenYellow, "F " + (int)cooldown);
                        BotFlash = false;
                    }
                    if (Mid != null)
                    {
                        var slot = Mid.GetSpellSlotFromName("summonerflash");

                        var cooldown = Mid.Spellbook.GetSpell(slot).CooldownExpires - Game.Time;
                        if (cooldown < 0)
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Red, "F rdy");
                            BotFlash = true;
                        }
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.GreenYellow, "F " + (int)cooldown);
                        BotFlash = false;
                    }
                    if (Jungle != null)
                    {
                        var slot = Jungle.GetSpellSlotFromName("summonerflash");

                        var cooldown = Jungle.Spellbook.GetSpell(slot).CooldownExpires - Game.Time;
                        if (cooldown < 0)
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.Red, "F rdy");
                            BotFlash = true;
                        }
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 30, color.GreenYellow, "F " + (int)cooldown);
                        BotFlash = false;
                    }
                    if (ADC != null)
                    {
                        var slot = ADC.GetSpellSlotFromName("summonerflash");

                        var cooldown = ADC.Spellbook.GetSpell(slot).CooldownExpires - Game.Time;
                        if (cooldown < 0)
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Red, "F rdy");
                            BotFlash = true;
                        }
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.GreenYellow, "F " + (int)cooldown);
                        BotFlash = false;
                    }
                    if (Support != null)
                    {
                        var slot = Support.GetSpellSlotFromName("summonerflash");

                        var cooldown = Support.Spellbook.GetSpell(slot).CooldownExpires - Game.Time;
                        if (cooldown < 0)
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Red, "F rdy");
                            BotFlash = true;
                        }
                        else
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 200, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.GreenYellow, "F " + (int)cooldown);
                        BotFlash = false;
                    }                   
                }
                           
                if (GankMenu["ward"].Cast<CheckBox>().CurrentValue)
                {
                    foreach (var ward in WardTracker.WardTracker.Wards)
                    {
                        if (Region.InTopWard(ward.WardObject))
                        {
                            TopWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Red, "Warded");
                        }
                        else
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos, color.Green, "Not Warded");
                            TopWard = false;
                        }
                        if (Region.InMidWard(ward.WardObject))
                        {
                            MidWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Red, "Warded");
                        }
                        else
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 15, color.Green, "Not Warded");
                            MidWard = false;
                        }
                        if (Region.InBotWard(ward.WardObject))
                        {
                            BotWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Red, "Warded");
                        }
                        else
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 45, color.Green, "Not Warded");
                            BotWard = false;
                        }
                        if (Region.InBotWard(ward.WardObject))
                        {
                            BotWard = true;
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Red, "Warded");
                        }
                        else
                        {
                            Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue + 300, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 60, color.Green, "Not Warded");
                            BotWard = false;
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
                    if (TopWard == true && MidWard == true && BotWard == true && TopFlash == false && BotFlash == false && MidFlash == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Top/Bot no flash!");
                    }
                    if (TopWard == true && MidWard == true && BotWard == true && MidFlash == false && BotFlash == true && TopFlash == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Mid/Top no flash!");
                    }
                    if (TopWard == true && MidWard == true && BotWard == true && BotFlash == false && TopFlash == true && MidFlash == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All warded! Bot/Mid no flash!");
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
                    if (TopFlash == true && BotFlash == true && MidFlash == true && BotWard == false && TopWard == false && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Bot/Top no ward!");
                    }
                    if (TopFlash == true && BotFlash == true && MidFlash == true && TopWard == false && BotWard == true && MidWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Top/Mid no ward!");
                    }
                    if (TopFlash == true && BotFlash == true && MidFlash == true && MidWard == false && TopWard == true && BotWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Yellow, "All flashs rdy! Mid/Bot no ward!");
                    }
                    if (TopFlash == false && TopWard == false && BotWard == true && BotFlash == true && MidFlash == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Top!!!");
                    }
                    if (BotFlash == false && BotWard == false && TopWard == true && TopFlash == true && MidFlash == true && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == true && TopFlash == true && BotFlash == true && BotWard == true)
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
                    if (TopFlash == false && TopWard == false && BotWard == true && BotFlash == false && MidFlash == false && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Top!!!");
                    }
                    if (BotFlash == false && BotWard == false && TopWard == true && TopFlash == false && MidFlash == false && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == true && TopFlash == false && BotFlash == false && BotWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == false && TopFlash == false && BotFlash == false && BotWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Top!!!");
                    }
                    if (MidFlash == false && MidWard == false && BotWard == false && BotFlash == false && TopFlash == false && TopWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Bot!!!");
                    }
                    if (BotWard == false && BotFlash == false && TopFlash == false && TopWard == false && MidFlash == false && MidWard == true)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot/Top!!!");
                    }
                    if (TopFlash == false && TopWard == false && BotWard == true && BotFlash == true && MidFlash == true && MidWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Top!!!");
                    }
                    if (BotFlash == false && BotWard == false && TopWard == true && TopFlash == true && MidFlash == true && MidWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Bot!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == true && TopFlash == true && BotFlash == true && BotWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid!!!");
                    }
                    if (MidFlash == false && MidWard == false && TopWard == false && TopFlash == false && BotFlash == true && BotWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Top!!!");
                    }
                    if (MidFlash == false && MidWard == false && BotWard == false && BotFlash == false && TopFlash == true && TopWard == false)
                    {
                        Drawing.DrawText(GankMenu["posX"].Cast<Slider>().CurrentValue, GankMenu["posY"].Cast<Slider>().CurrentValue + gapPos + 75, color.Green, "Gank Mid/Bot!!!");
                    }
                    if (BotWard == false && BotFlash == false && TopFlash == false && TopWard == false && MidFlash == true && MidWard == false)
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

  

