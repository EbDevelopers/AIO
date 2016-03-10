using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AJS.Utility.Junglesystem
{
    public class Jungle
    {
        public static List<Camp> Camps;

        static Jungle()
        {
            Camps = new List<Camp>
                {
                    // Order: Blue
                    new Camp(
                        100, 300, new Vector3(3800.99f, 7883.53f, 52.18f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Blue1.1.1", true), new Mob("SRU_BlueMini1.1.2"),
                                new Mob("SRU_BlueMini21.1.3")
                            }), true,
                        GameObjectTeam.Order),
                    //Order: Wolves
                    new Camp(
                        100, 100, new Vector3(3849.95f, 6504.36f, 52.46f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf2.1.1", true), new Mob("SRU_MurkwolfMini2.1.2"),
                                new Mob("SRU_MurkwolfMini2.1.3")
                            }), false,
                        GameObjectTeam.Order),
                    //Order: Chicken
                    new Camp(
                        100, 100, new Vector3(6943.41f, 5422.61f, 52.62f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak3.1.1", true), new Mob("SRU_RazorbeakMini3.1.2"),
                                new Mob("SRU_RazorbeakMini3.1.3"), new Mob("SRU_RazorbeakMini3.1.4")
                            }), false,
                         GameObjectTeam.Order),
                    //Order: Red
                    new Camp(
                        100, 300, new Vector3(7813.07f, 4051.33f, 53.81f),
                        new List<Mob>(
                            new[]
                            { new Mob("SRU_Red4.1.1", true), new Mob("SRU_RedMini4.1.2"), new Mob("SRU_RedMini4.1.3") }),
                        true,  GameObjectTeam.Order),
                    //Order: Krug
                    new Camp(
                        100, 100, new Vector3(8370.58f, 2718.15f, 51.09f),
                        new List<Mob>(new[] { new Mob("SRU_Krug5.1.2", true), new Mob("SRU_KrugMini5.1.1") }), false,
                         GameObjectTeam.Order),
                    //Order: Gromp
                    new Camp(
                        100, 100, new Vector3(2164.34f, 8383.02f, 51.78f),
                        new List<Mob>(new[] { new Mob("SRU_Gromp13.1.1", true) }), false,
                         GameObjectTeam.Order),
                    //Chaos: Blue
                    new Camp(
                        100, 300, new Vector3(10984.11f, 6960.31f, 51.72f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Blue7.1.1", true), new Mob("SRU_BlueMini7.1.2"),
                                new Mob("SRU_BlueMini27.1.3")
                            }), true,
                        GameObjectTeam.Chaos),
                    //Chaos: Wolves
                    new Camp(
                        100, 100, new Vector3(10983.83f, 8328.73f, 62.22f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Murkwolf8.1.1", true), new Mob("SRU_MurkwolfMini8.1.2"),
                                new Mob("SRU_MurkwolfMini8.1.3")
                            }), false,
                        GameObjectTeam.Chaos),
                    //Chaos: Chicken
                    new Camp(
                        100, 100, new Vector3(7852.38f, 9562.62f, 52.30f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Razorbeak9.1.1", true), new Mob("SRU_RazorbeakMini9.1.2"),
                                new Mob("SRU_RazorbeakMini9.1.3"), new Mob("SRU_RazorbeakMini9.1.4")
                            }), false,
                         GameObjectTeam.Chaos),
                    //Chaos: Red
                    new Camp(
                        100, 300, new Vector3(7139.29f, 10779.34f, 56.38f),
                        new List<Mob>(
                            new[]
                            {
                                new Mob("SRU_Red10.1.1", true), new Mob("SRU_RedMini10.1.2"), new Mob("SRU_RedMini10.1.3")
                            }), true,  GameObjectTeam.Chaos),
                    //Chaos: Krug
                    new Camp(
                        100, 100, new Vector3(6476.17f, 12142.51f, 56.48f),
                        new List<Mob>(new[] { new Mob("SRU_Krug11.1.2", true), new Mob("SRU_KrugMini11.1.1") }), false,
                         GameObjectTeam.Chaos),
                    //Chaos: Gromp
                    new Camp(
                        100, 100, new Vector3(12671.83f, 6306.60f, 51.71f),
                        new List<Mob>(new[] { new Mob("SRU_Gromp14.1.1", true) }), false,
                         GameObjectTeam.Chaos),
                    //Neutral: Dragon
                    new Camp(
                        150, 360, new Vector3(9813.83f, 4360.19f, -71.24f),
                        new List<Mob>(new[] { new Mob("SRU_Dragon6.1.1", true) }), true,
                         GameObjectTeam.Neutral),
                    //Neutral: Rift Herald
                    new Camp(
                        240, 300, new Vector3(4993.14f, 10491.92f, -71.24f),
                        new List<Mob>(new[] { new Mob("SRU_RiftHerald", true) }), true,
                         GameObjectTeam.Neutral),
                    //Neutral: Baron
                    new Camp(
                        1200, 420, new Vector3(4993.14f, 10491.92f, -71.24f),
                        new List<Mob>(new[] { new Mob("SRU_Baron12.1.1", true) }), true,
                         GameObjectTeam.Neutral),
                    //Dragon: Crab
                    new Camp(
                        150, 180, new Vector3(10647.70f, 5144.68f, -62.81f),
                        new List<Mob>(new[] { new Mob("SRU_Crab15.1.1", true) }), false,
                         GameObjectTeam.Neutral),
                    //Baron: Crab
                    new Camp(
                        150, 180, new Vector3(4285.04f, 9597.52f, -67.60f),
                        new List<Mob>(new[] { new Mob("SRU_Crab16.1.1", true) }), false,
                         GameObjectTeam.Neutral),
                };
        }


        public class Camp
        {
            public Camp(float spawnTime,
                float respawnTime,
                Vector3 position,
                List<Mob> mobs,
                bool isBig,
                GameObjectTeam team) 
            {
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                Position = position;
                MinimapPosition = Drawing.WorldToMinimap(Position);
                Mobs = mobs;
                IsBig = isBig;
                Team = team;
            }

            public float SpawnTime { get; set; }
            public float RespawnTime { get; private set; }
            public Vector3 Position { get; private set; }
            public Vector2 MinimapPosition { get; private set; }
            public List<Mob> Mobs { get; private set; }
            public bool IsBig { get; set; }
            public GameObjectTeam Team { get; set; }
        }

        public class Mob
        {
            public Mob(string name, bool isBig = false) 
            {
                Name = name;
                IsBig = isBig;
            }

            public string Name { get; private set; }
            public bool IsBig { get; set; }
        }
    }
}