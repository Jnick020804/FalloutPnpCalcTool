using FalloutPnpCalcTool.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalloutPnpCalcTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static FalloutPnpCalcDB database;
        public static List<CompositeBeing> Beings { get; set; }
        public static List<Character> Characters { get; set; }
        public static List<Beast> Beasts { get; set; }
        public static List<Weapon> Weapons { get; set; }
        public static List<Attack> Attacks { get; set; }
        public static DateTime BeingDate { get; set; }
        public static DateTime CharacterDate { get; set; }
        public static DateTime BeastDate { get; set; }
        public static DateTime WeaponDate { get; set; }
        public static DateTime AttackDate { get; set; }

        public static FalloutPnpCalcDB Database
        {

            get
            {
                if (database == null)
                {

                    database = new FalloutPnpCalcDB();
                }
                return database;

            }
        }

        public App()
        {
            List<Character> c = new List<Character>();
            List<Beast> b = new List<Beast>();
            List<Weapon> w = new List<Weapon>();
            List<Attack> a = new List<Attack>();
            Beings = new List<CompositeBeing>();
            Characters = new List<Character>();
            Beasts = new List<Beast>();
            Weapons = new List<Weapon>();
            Attacks = new List<Attack>();
            Task t1 = Task.Run(async () => { c = await App.Database.GetCharactersAsync(); });
            Task t2 = Task.Run(async () => { b = await App.Database.GetBeastsAsync(); });
            Task t3 = Task.Run(async () => { w = await App.Database.GetWeaponsAsync(); });
            Task t4 = Task.Run(async () => { a = await App.Database.GetAttacksAsync(); });


            Task.WaitAll(t1, t2, t3, t4);
            Characters = c;
            CharacterDate = DateTime.Now;

            Beasts = b;
            BeastDate = DateTime.Now;

            Weapons = w;
            WeaponDate = DateTime.Now;

            Attacks = a;
            AttackDate = DateTime.Now;

            foreach(var item in c)
            {
                CompositeBeing cb = new CompositeBeing();
                cb.Name = item.Name;
                cb.Type = item.GetType();
                cb.Object = item;

                Beings.Add(cb);
            }
            BeingDate = DateTime.Now;
        }
    }
}
