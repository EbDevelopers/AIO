using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace WardTracker
{
    class WardData
    {
        public float Duration = int.MaxValue;
        public string ObjectName = "visionward";
        public float Range = 1100;
        public string SpellName = "visionward";
    }

    class WardDatabase
    {
        // ReSharper disable once InconsistentNaming
        public static List<WardData> wardDatabase = new List<WardData>();
        public static Dictionary<string, WardData> WardspellNames = new Dictionary<string, WardData>();
        public static Dictionary<string, WardData> WardObjNames = new Dictionary<string, WardData>();
        public static WardData MissileWardData;

        static WardDatabase()
        {
            LoadWardDatabase();
            LoadWardDictionary();
        }

        private static void LoadWardDictionary()
        {
            foreach (var ward in wardDatabase)
            {
                var spellName = ward.SpellName.ToLower();
                if (!WardspellNames.ContainsKey(spellName))
                {
                    WardspellNames.Add(spellName, ward);
                }

                var objName = ward.ObjectName.ToLower();
                if (!WardObjNames.ContainsKey(objName))
                {
                    WardObjNames.Add(objName, ward);
                }
            }
        }

        private static void LoadWardDatabase()
        {
            //Trinkets:
            wardDatabase.Add(
            new WardData
            {
                Duration = 1 * 60 * 1000,
                ObjectName = "YellowTrinket",
                Range = 1100,
                SpellName = "TrinketTotemLvl1",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = 2 * 60 * 1000,
                ObjectName = "YellowTrinketUpgrade",
                Range = 1100,
                SpellName = "TrinketTotemLvl2",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = int.MaxValue,
                ObjectName = "VisionWard",
                Range = 1100,
                SpellName = "VisionWard",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = int.MaxValue,
                ObjectName = "BlueTrinket",
                Range = 1100,
                SpellName = "TrinketOrbLvl3",
            });


            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "TrinketTotemLvl3",
            });
            //Ward items and normal wards:
            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "SightWard",
            });

            wardDatabase.Add(
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "ItemGhostWard",
            });

            MissileWardData =
            new WardData
            {
                Duration = 3 * 60 * 1000,
                ObjectName = "MissileWard",
                Range = 1100,
                SpellName = "MissileWard",
            };
        }
    }
    class WardTrackerInfo
    {
        public WardData WardData;
        public Vector3 Position;
        public Obj_AI_Base WardObject;
        public float Timestamp;
        public float EndTime;
        public bool UnknownDuration;
        public Vector2 StartPos;
        public Vector2 EndPos;

        public WardTrackerInfo(
            WardData wardData,
            Vector3 position,
            Obj_AI_Base wardObject,
            bool fromMissile = false,
            float timestamp = 0)
        {
            WardData = wardData;
            Position = position;
            WardObject = wardObject;
            UnknownDuration = fromMissile;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Timestamp = timestamp == 0 ? Environment.TickCount : timestamp;
            EndTime = Timestamp + wardData.Duration;
        }

    }

    class WardTracker
    {
        public static List<WardTrackerInfo> Wards = new List<WardTrackerInfo>();
        public static float LastCheckExpiredWards;

        public static System.Drawing.Size StrSize = GetTextExtent("00:00");
        public static System.Drawing.Size UtStrSize = GetTextExtent("?? 00:00 ??");
        public static System.Drawing.Size WardStrSize = GetTextExtent("Ward");

        public static Menu Menu, lala;

        public static void OnLoad()
        {
            Menu = MainMenu.AddMenu("ward.system", "Ward System");
            {
                lala = Menu.AddSubMenu("pb", "blbl");
                lala.Add("track", new CheckBox("Tack enemy warsd"));
            }

            Obj_AI_Base.OnProcessSpellCast += Game_OnProcessSpell;
            GameObject.OnCreate += Game_OnCreateObj;
            GameObject.OnDelete += Game_OnDeleteObj;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            InitWards();
        }


        static WardTracker()
        {
            LastCheckExpiredWards = 0;
        }

        public static System.Drawing.Size GetTextExtent(string text)
        {
            if (text != null)
            {
                return Drawing.GetTextEntent(text, 5);
            }
            else
            {
                return Drawing.GetTextEntent("A", 5);
            }
        }
        public static void DrawText(float x, float y, Color c, string text)
        {
            if (text != null)
            {
                Drawing.DrawText(x, y, c, text);
            }
        }
        public static string FormatTime(float time)
        {
            if (time > 0)
            {
                return Convert.ToString(time, CultureInfo.InvariantCulture);
            }
            else
            {
                return "00.00";
            }
        }
        private static void InitWards()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>())
            {
                if (obj != null && obj.IsValid && !obj.IsDead)
                {
                    Game_OnCreateObj(obj, null);
                }
            }
        }

        private static void Game_OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (!lala["track"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (sender.Type == GameObjectType.obj_AI_Minion)
            {
                foreach (var ward in Wards)
                {
                    if (ward.WardObject != null && ward.WardObject.NetworkId == sender.NetworkId)
                    {
                        var ward1 = ward;
                        Core.DelayAction(() => Wards.Remove(ward1), 0);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!lala["track"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (Environment.TickCount > LastCheckExpiredWards)
            {
                CheckExpiredWards();
                LastCheckExpiredWards = Environment.TickCount + 500;
            }

        }

        private static void CheckExpiredWards()
        {
            foreach (var ward in Wards)
            {
                if (Environment.TickCount > ward.EndTime)
                {
                    var ward1 = ward;
                    Core.DelayAction(() => Wards.Remove(ward1), 0);
                }

                if (ward.WardObject != null && (ward.WardObject.IsDead || !ward.WardObject.IsValid))
                {
                    var ward1 = ward;
                    Core.DelayAction(() => Wards.Remove(ward1), 0);
                }
            }
        }

        private static void Game_OnProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {

            if (!lala["track"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (hero.IsEnemy && hero is AIHeroClient && hero.IsValid())
            {
                WardData wardData;
                if (WardDatabase.WardspellNames.TryGetValue(args.SData.Name.ToLower(), out wardData))
                {
                    var pos = args.End.To2D();

                    Core.DelayAction(() =>
                    {
                        if (Wards.Any(ward => ward.Position.To2D().Distance(pos) < 50
                                              && Environment.TickCount - ward.Timestamp < 100))
                        {
                            return;
                        }

                        var newWard = new WardTrackerInfo(
                            wardData,
                            pos.To3D(),
                            null
                            )
                        {
                            StartPos = args.Start.To2D(),
                            EndPos = args.End.To2D()
                        };
                        Wards.Add(newWard);
                    } , 50);
                }
            }
        }

        private static void Game_OnCreateObj(GameObject sender, EventArgs args)
        {
            /*if (sender.Name.ToLower().Contains("minion")
                || sender.Name.ToLower().Contains("turret"))
            {
                return;
            }
            if (sender.IsValid<MissileClient>())
            {
                var tMissile = sender as MissileClient;
                if (tMissile.SpellCaster.Type != GameObjectType.obj_AI_Hero)
                {
                    return;
                }
            }
            ConsolePrinter.Print(sender.Type + " : " + sender.Name);*/

            if (!lala["track"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            //Visible ward placement
            var obj = sender as Obj_AI_Minion;
            WardData wardData;

            if (obj != null && obj.IsEnemy
                && WardDatabase.WardObjNames.TryGetValue(obj.CharData.BaseSkinName.ToLower(), out wardData))
            {
                var timestamp = Environment.TickCount - (obj.MaxMana - obj.Mana) * 1000;

                WardTrackerInfo newWard = new WardTrackerInfo(
                            wardData,
                            obj.Position,
                            obj,
                            !obj.IsVisible && args == null,
                            timestamp
                            );

                Wards.Add(newWard);

                Core.DelayAction(() =>
                {
                    if (newWard.WardObject != null && newWard.WardObject.IsValid && !newWard.WardObject.IsDead)
                    {
                        timestamp = Environment.TickCount - (obj.MaxMana - obj.Mana) * 1000;

                        newWard.Timestamp = timestamp;

                        foreach (var ward in Wards)
                        {
                            if (ward.WardObject == null)
                            {
                                //Check for Process Spell wards
                                if (ward.Position.Distance(sender.Position) < 550
                                        && Math.Abs(ward.Timestamp - timestamp) < 2000)
                                {
                                    var ward1 = ward;
                                    Core.DelayAction(() => Wards.Remove(ward1), 0);
                                    break;
                                }
                            }
                        }
                    }
                }, 500);

            }

            //FOW placement
            var missile = sender as MissileClient;

            if (missile != null && missile.SpellCaster.IsEnemy)
            {
                if (missile.SData.Name.ToLower() == "itemplacementmissile")// && !missile.SpellCaster.IsVisible)
                {
                    var dir = (missile.EndPosition.To2D() - missile.StartPosition.To2D()).Normalized();
                    var pos = missile.StartPosition.To2D() + dir * 500;

                    if (Wards.Any(ward => ward.Position.To2D().Distance(pos) < 750
                                          && Environment.TickCount - ward.Timestamp < 50))
                    {
                        return;
                    }

                    Core.DelayAction(() =>
                    {
                        if (Wards.Any(ward => ward.Position.To2D().Distance(pos) < 750
                                              && Environment.TickCount - ward.Timestamp < 125))
                        {
                            return;
                        }

                        var newWard = new WardTrackerInfo(
                            WardDatabase.MissileWardData,
                            pos.To3D(),
                            null,
                            true
                            )
                        {
                            StartPos = missile.StartPosition.To2D(),
                            EndPos = missile.EndPosition.To2D()
                        };


                        Wards.Add(newWard);
                    }, 100);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!lala["track"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            foreach (var ward in Wards)
            {
                var wardPos = ward.WardObject != null ? ward.WardObject.Position : ward.Position;

                if (wardPos.IsOnScreen() && ward.EndTime > Environment.TickCount)
                {
                    var wardScreenPos = Drawing.WorldToScreen(wardPos);
                    var timeStr = FormatTime((ward.EndTime - Environment.TickCount) / 1000f);
                    var tSize = StrSize;

                    if (timeStr != null)
                    {
                        if (ward.WardData.ObjectName == "VisionWard")
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.DeepPink);
                            Drawing.DrawCircle(Drawing.WorldToMinimap(wardPos).To3D(), 20, Color.DeepPink);
                            // ReSharper disable once PossibleLossOfFraction
                            Drawing.DrawText(wardScreenPos.X - WardStrSize.Width / 2, wardScreenPos.Y, Color.DeepPink, "Pink");
                        }
                        else if (ward.WardData.ObjectName == "BlueTrinket")
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.DodgerBlue);
                            Drawing.DrawCircle(Drawing.WorldToMinimap(wardPos).To3D(), 20, Color.DodgerBlue);
                            // ReSharper disable once PossibleLossOfFraction
                            Drawing.DrawText(wardScreenPos.X - WardStrSize.Width / 2, wardScreenPos.Y, Color.DodgerBlue, "Blue");
                        }
                        else
                        {
                            Drawing.DrawCircle(wardPos, 100, Color.LawnGreen);
                            Drawing.DrawCircle(Drawing.WorldToMinimap(wardPos).To3D(), 20, Color.LawnGreen);

                            // ReSharper disable once PossibleLossOfFraction
                            Drawing.DrawText(wardScreenPos.X - WardStrSize.Width / 2, wardScreenPos.Y, Color.White, "Ward");
                            // ReSharper disable once PossibleLossOfFraction
                            Drawing.DrawText(wardScreenPos.X - tSize.Width / 2, wardScreenPos.Y + WardStrSize.Height,
                                Color.LawnGreen, "" + timeStr);
                        }

                    }
                }


            }
        }
    }
}