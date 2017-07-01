using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FalloutPnpCalcTool.Models
{
    public class FalloutPnpCalcDB
    {
        readonly SQLiteAsyncConnection database;

        public FalloutPnpCalcDB()
        {
            database = new SQLiteAsyncConnection("FalloutCalc.db");
            database.CreateTableAsync<Character>().Wait();
            database.CreateTableAsync<PlayerWeapon>().Wait();
            database.CreateTableAsync<Weapon>().Wait();
            database.CreateTableAsync<Beast>().Wait();
            database.CreateTableAsync<BeastAttack>().Wait();
            database.CreateTableAsync<Attack>().Wait();
        }

        public Task<List<Character>> GetCharactersAsync()
        {
            return database.Table<Character>().ToListAsync();
        }

        public Task<Character> GetCharacterAsync(Guid ID)
        {
            return database.Table<Character>().Where(c => c.CharacterID == ID).FirstOrDefaultAsync();
        }

        public Task<List<Beast>> GetBeastsAsync()
        {
            return database.Table<Beast>().ToListAsync();
        }

        public Task<Beast> GetBeastAsync(Guid id)
        {
            return database.Table<Beast>().Where(b => b.ID == id).FirstOrDefaultAsync();
        }

        public Task<List<Weapon>> GetWeaponsAsync()
        {
            return database.Table<Weapon>().ToListAsync();
        }

        public Task<Weapon> GetWeaponAsync(Guid id)
        {
            return database.Table<Weapon>().Where(w => w.ID == id).FirstOrDefaultAsync();
        }

        public Task<List<Attack>> GetAttacksAsync()
        {
            return database.Table<Attack>().ToListAsync();
        }

        public async Task<List<Weapon>> GetWeaponsForCharacterAsync(Guid id)
        {
            var playerWeapons = await database.Table<PlayerWeapon>().Where(t => t.CharacterID == id).ToListAsync();

            List<Weapon> w = await database.Table<Weapon>().ToListAsync();

            return w.Where(we => playerWeapons.Any(p => p.WeaponID == we.ID)).ToList();
        }

        public async Task<List<Attack>> GetAttacksForBeastAsync(Guid id)
        {
            var beastAttacks = await database.Table<BeastAttack>().Where(t => t.BeastID == id).ToListAsync();

            return await database.Table<Attack>().Where(t => beastAttacks.Any(l => l.AttackID == t.ID)).ToListAsync();
        }

        public int SaveCharacter(Character c)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(c); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(c); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    UpdateBeings();
                    UpdateCharacters();
                    return inserted;
                }
                else
                {
                    UpdateBeings();
                    UpdateCharacters();
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        private void UpdateBeings()
        {
            List<Character> c = new List<Character>();
            List<Beast> b = new List<Beast>();

            Task t1 = Task.Run(async () => { c = await GetCharactersAsync(); });
            Task t2 = Task.Run(async () => { b = await GetBeastsAsync(); });

            Task.WaitAll(t1, t2);
            List<CompositeBeing> cbl = new List<CompositeBeing>();

            foreach(var item in c)
            {
                CompositeBeing cb = new CompositeBeing();
                cb.Name = item.Name;
                cb.Type = item.GetType();
                cb.Object = item;

                cbl.Add(cb);
            }

            foreach(var item in b)
            {
                CompositeBeing cb = new CompositeBeing();
                cb.Name = item.Name;
                cb.Type = item.GetType();
                cb.Object = item;

                cbl.Add(cb);
            }

            App.Beings = cbl;
            App.BeingDate = DateTime.Now;
        }

        private void UpdateCharacters()
        {
            List<Character> c = new List<Character>();
            Task t1 = Task.Run(async () => { c = await GetCharactersAsync(); });
            t1.Wait();

            App.Characters = c;
            App.CharacterDate = DateTime.Now;
        }

        public int SaveBeast(Beast b)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(b); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(b); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    UpdateBeasts();
                    UpdateBeings();
                    return inserted;
                }
                else
                {
                    UpdateBeasts();
                    UpdateBeings();
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void UpdateBeasts()
        {
            List<Beast> w = new List<Beast>();

            Task t1 = Task.Run(async () => { w = await GetBeastsAsync(); });
            t1.Wait();

            App.Beasts = w;
            App.BeastDate = DateTime.Now;
        }

        public int SaveAttack(Attack a)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(a); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(a); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    return inserted;
                }
                else
                {
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int SaveWeapon(Weapon w)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(w); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(w); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    UpdateWeapons();
                    return inserted;
                }
                else
                {
                    UpdateWeapons();
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private void UpdateWeapons()
        {
            List<Weapon> w = new List<Weapon>();

            Task t1 = Task.Run(async () => { w = await GetWeaponsAsync(); });
            t1.Wait();

            App.Weapons = w;
            App.WeaponDate = DateTime.Now;
        }

        public int SavePlayerWeapon(PlayerWeapon pw)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(pw); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(pw); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    return inserted;
                }
                else
                {
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int SaveBeastAttack(BeastAttack ba)
        {

            try
            {
                int updated = 0;
                Task T1 = Task.Run(async () => { updated = await database.UpdateAsync(ba); });
                while (!T1.IsCompleted)
                {
                    continue;
                }

                if (updated == 0)
                {
                    int inserted = 0;
                    Task T2 = Task.Run(async () => { inserted = await database.InsertAsync(ba); });
                    while (!T2.IsCompleted)
                    {
                        continue;
                    }
                    return inserted;
                }
                else
                {
                    return updated;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteCharacter(Character c)
        {
            try
            {
                List<Character> deleted = new List<Character>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Character>("Delete from Character Where CharacterID = ?", c.CharacterID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateBeings();
                UpdateCharacters();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        public int DeleteCharacter(Guid c)
        {
            try
            {
                List<Character> deleted = new List<Character>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Character>("Delete from Character Where CharacterID = ?", c); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateBeings();
                UpdateCharacters();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteBeast(Beast b)
        {
            try
            {
                List<Beast> deleted = new List<Beast>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Beast>("Delete from Beast Where ID = ?", b.ID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateBeasts();
                UpdateBeings();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public int DeleteBeast(Guid b)
        {
            try
            {
                List<Beast> deleted = new List<Beast>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Beast>("Delete from Beast Where ID = ?", b); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateBeasts();
                UpdateBeings();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteAttack(Attack a)
        {
            try
            {
                List<Attack> deleted = new List<Attack>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Attack>("Delete from Attack Where ID = ?", a.ID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteWeapon(Weapon w)
        {
            try
            {
                List<Weapon> deleted = new List<Weapon>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Weapon>("Delete from Weapon Where ID = ?", w.ID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateWeapons();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteWeapon(Guid w)
        {
            try
            {
                List<Weapon> deleted = new List<Weapon>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<Weapon>("Delete from Weapon Where ID = ?", w); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                UpdateWeapons();
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeletePlayerWeapon(PlayerWeapon pw)
        {
            try
            {
                List<PlayerWeapon> deleted = new List<PlayerWeapon>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<PlayerWeapon>("Delete from PlayerWeapon Where ID = ?", pw.ID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeletePlayerWeapon(Guid pw)
        {
            try
            {
                List<PlayerWeapon> deleted = new List<PlayerWeapon>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<PlayerWeapon>("Delete from PlayerWeapon Where WeaponID = ?", pw); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int DeleteBeastAttack(BeastAttack ba)
        {
            try
            {
                List<BeastAttack> deleted = new List<BeastAttack>();
                Task t1 = Task.Run(async () => { deleted = await database.QueryAsync<BeastAttack>("Delete from BeastAttack Where ID = ?", ba.ID); });
                while (!t1.IsCompleted)
                {
                    continue;
                }
                return deleted.Count;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }
    }
    public enum GoverningSkill
    {
        SMALLGUNS,
        MELEE,
        UNARMED,
        THROWN,
        ENERGYWEAPONS,
        BIGGUNS
    }

    public class Character
    {
        [PrimaryKey]
        public Guid CharacterID { get; set; }
        public int Perception { get; set; }
        public int SmallGuns { get; set; }
        public int Melee { get; set; }
        public int Unarmed { get; set; }
        public int Thrown { get; set; }
        public int EnergyWeapons { get; set; }
        public int BigGuns { get; set; }
        public int ArmorClass { get; set; }
        public string Name { get; set; }
        [Ignore]
        public List<Weapon> Weapons { get; set; }
    }

    public class CompositeBeing
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Object { get; set; }
    }
    public class PlayerWeapon
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        [ForeignKey("Character")]
        public Guid CharacterID { get; set; }
        [ForeignKey("Weapon")]
        public Guid WeaponID { get; set; }
    }

    public class Weapon
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public int WeaponDice { get; set; }
        public int NumberOfDice { get; set; }
        public int RangeFeet { get; set; }
        public int WeaponModifier { get; set; }
        public string WeaponName { get; set; }
        public GoverningSkill skill { get; set; }
    }

    public class Beast
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public int Perception { get; set; }
        public int ArmorClass { get; set; }
        public string Name { get; set; }
        [Ignore]
        public List<BeastAttack> Attacks { get; set; }
    }

    public class BeastAttack
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        [ForeignKey("Beast")]
        public Guid BeastID { get; set; }
        [ForeignKey("Attack")]
        public Guid AttackID { get; set; }
    }

    public class Attack
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public int NumberOfDice { get; set; }
        public int WeaponDice { get; set; }
        public int WeaponModifier { get; set; }
        public int HitChance { get; set; }
        public string Name { get; set; }
    }

    
}