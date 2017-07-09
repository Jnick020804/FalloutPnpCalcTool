using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutPnpCalcTool.Models
{
    public class Result
    {
        public string text { get; set; }
        public DateTime timeStamp { get; set; }

        public string ResultString
        {
            get
            {
                if (timeStamp == DateTime.MaxValue)
                {
                    return $"{text}";
                }
                else
                {
                    return $"{text} {timeStamp.ToString("hh:mm:ss:fff")}";
                }
            }
        }

        public Result(string t, DateTime ts)
        {
            this.text = t;
            this.timeStamp = ts;
        }
    }

    public class BattleSimulationEngine
    {
        List<BattleObject> PlayerTeam { get; set; }
        List<BattleObject> EnemyTeam { get; set; }
        List<BattleObject> TurnOrder { get; set; }
        int LightPenalty { get; set; }
        public List<Result> Results { get; set; }
        public List<Result> PlayerDeaths { get; set; }
        public List<Result> EnemyDeaths { get; set; }
        BattleObject current { get; set; }
        public List<Attack> currentAttack { get; set; }
        public List<Weapon> currentWeapon { get; set; }
        public BattleObjectComparison currentTarget { get; set; }
        public Random Random { get; set; }

        
        public BattleSimulationEngine(List<BattleObject> players, List<BattleObject> enemies, int lp)
        {
            this.LightPenalty = lp;
            Random = new Random();
            PlayerTeam = new List<BattleObject>();
            EnemyTeam = new List<BattleObject>();
            TurnOrder = new List<BattleObject>();
            Results = new List<Result>();
            PlayerDeaths = new List<Result>();
            EnemyDeaths = new List<Result>();

            EnemyTeam = enemies;
            PlayerTeam = players;

            SetComparisions();

            TurnOrder.AddRange(PlayerTeam);
            TurnOrder.AddRange(EnemyTeam);
            TurnOrder = TurnOrder.OrderByDescending(e => e.Sequence).ToList();
        }

        public void SetComparisions()
        {
            // set comparisons
            foreach (var item in PlayerTeam)
            {
                item.Comparisons = new List<BattleObjectComparison>();

                foreach(var item2 in EnemyTeam)
                {
                    BattleObjectComparison b = new BattleObjectComparison();
                    b.Self = item;
                    b.Target = item2;

                    item.Comparisons.Add(b);
                }
            }


            foreach (var item in EnemyTeam)
            {
                item.Comparisons = new List<BattleObjectComparison>();

                foreach (var item2 in PlayerTeam)
                {
                    BattleObjectComparison b = new BattleObjectComparison();
                    b.Self = item;
                    b.Target = item2;

                    item.Comparisons.Add(b);
                }
            }
        }

        public void MoveObjectTowards(Guid PlayerID, BattleObjectComparison target, int numberHexes)
        {
            bool isPlayer = this.PlayerTeam.Any(e => e.ID == PlayerID);
            bool targetIsPlayer = this.PlayerTeam.Any(e => e.ID == target.Target.ID);
            BattleObject Player;
            BattleObject Target;
            
            int PlayerIndex = 0;
            if (isPlayer)
            {
                PlayerIndex = this.PlayerTeam.IndexOf(this.PlayerTeam.First(e => e.ID == PlayerID));
                Player = this.PlayerTeam.First(e => e.ID == PlayerID);

            }
            else
            {
                PlayerIndex = this.EnemyTeam.IndexOf(this.EnemyTeam.First(e => e.ID == PlayerID));
                Player = this.EnemyTeam.First(e => e.ID == PlayerID);
            }

            if(targetIsPlayer)
            {
                Target = this.PlayerTeam.First(e => e.ID == target.Target.ID);
            }
            else
            {
                Target = this.EnemyTeam.First(e => e.ID == target.Target.ID);
            }

            string resText = $"{Player.Name} starts at ({Player.XCord},{Player.YCord})";

            /*F(t) = (1-t)(x1,y1) + t(x2,y2)
             * Therfore F(x/d) will yeild the point that is x spaces toward B from a
            */
            double t = numberHexes / target.DistanceFeet;
            double oneMinusT = (1 - t);

            double xCordA = Player.XCord * oneMinusT;
            double yCordA = Player.YCord * oneMinusT;
            double xCordB = Target.XCord * t;
            double yCordB = Target.YCord * t;

            Player.XCord = ((int)Math.Round(xCordA + xCordB));
            Player.YCord = ((int)Math.Round(yCordA + yCordB));

            resText = $"{resText} {current.Name} moves {numberHexes} hexes towards {Target.Name} at ({Target.XCord},{Target.YCord}) and ends up at ({Player.XCord},{Player.YCord})";
            this.Results.Add(new Result(resText,DateTime.Now));
            //if(isPlayer)
            //{
            //    PlayerTeam.RemoveAt(PlayerIndex);
            //    PlayerTeam.Insert(PlayerIndex, Player);
            //}
            //else
            //{
            //    EnemyTeam.RemoveAt(PlayerIndex);
            //    EnemyTeam.Insert(PlayerIndex, Player);
            //}
            //string resText = $"{current.Name} moves {numberHexes} hexes towards {Target.Name}";
            //this.Results.Add(resText);
            //SetComparisions();

        }

        public void CurrentTakeAction()
        {
           while(current.ActionPoints > 0)
            {

                    if (current.Type == typeof(Character))
                    {
                        Character c = (Character)current.Object;
                        if (currentTarget == null)
                            currentTarget = current.Comparisons.OrderBy(e => e.DistanceFeet).FirstOrDefault();

                        if(currentTarget == null)
                        {
                            break;
                        }
                        List<Weapon> w = new List<Weapon>();
                        if (currentWeapon == null)
                        {
                            Task t1 = Task.Run(async () => { w = await App.Database.GetWeaponsForCharacterAsync(c.CharacterID); });
                            t1.Wait();
                            currentWeapon = w;
                        }

                        int index = Random.Next(0, currentWeapon.Count);
                        Weapon we = currentWeapon.ElementAt(index);

                        if (we.APCost == 0)
                        {
                            we.APCost = 4;
                        }


                        if (we.RangeFeet < currentTarget.DistanceFeetRounded)
                        {
                            bool distanceGreaterThanMovement = (currentTarget.DistanceFeet - current.ActionPoints) > we.RangeFeet;
                            int ACToUse = distanceGreaterThanMovement ? current.ActionPoints : (currentTarget.DistanceFeetRounded - we.RangeFeet);
                            MoveObjectTowards(current.ID, currentTarget, ACToUse);
                            current.ActionPoints -= ACToUse;
                            continue;
                        }
                        else
                        {
                            if (we.APCost < current.ActionPoints)
                            {
                                MakeCharacterAttackWithWeapon(c, we, currentTarget);
                            }
                            else
                            {
                                current.ActionPoints = 0;
                            }
                            continue;
                        }


                    }
                    else
                    {
                        Beast c = (Beast)current.Object;
                        if (currentTarget == null)
                            currentTarget = current.Comparisons.OrderBy(e => e.DistanceFeet).FirstOrDefault();

                        if(currentTarget == null)
                        {
                            break;
                        }
                        List<Attack> w = new List<Attack>();
                        if (currentAttack == null)
                        {
                            Task t1 = Task.Run(async () => { w = await App.Database.GetAttacksForBeastAsync(c.ID); });
                            t1.Wait();
                            currentAttack = w;
                        }

                        int index = Random.Next(0, currentAttack.Count);
                        Attack we = currentAttack.ElementAt(index);
                        if (we.RangeFeet == 0)
                        {
                            we.RangeFeet = 5;
                        }
                        if (we.RangeFeet < currentTarget.DistanceFeetRounded)
                        {
                            bool distanceGreaterThanMovement = (currentTarget.DistanceFeet - current.ActionPoints) > we.RangeFeet;
                            int ACToUse = distanceGreaterThanMovement ? current.ActionPoints : (currentTarget.DistanceFeetRounded - we.RangeFeet);
                            MoveObjectTowards(current.ID, currentTarget, ACToUse);
                            current.ActionPoints -= ACToUse;
                            continue;
                        }
                        else
                        {
                            if (we.APCost == 0)
                            {
                                we.APCost = 4;
                            }

                            if (we.APCost < current.ActionPoints)
                            {
                                MakeBeastAttack(c, we, currentTarget);
                            }
                            else
                            {
                                current.ActionPoints = 0;
                            }
                            continue;
                        }
                    }
                }
            
        }

        public void MakeCharacterAttackWithWeapon(Character player, Weapon we, BattleObjectComparison target)
        {
           
            bool targetIsPlayer = this.PlayerTeam.Any(e => e.ID == target.Target.ID);
            int BaseSkill = 0;
            string resText = $"{current.Name} attacks {target.Target.Name} with {we.WeaponName}";
            switch (we.skill)
            {
                case GoverningSkill.SMALLGUNS:
                    BaseSkill = player.SmallGuns;
                    break;
                case GoverningSkill.BIGGUNS:
                    BaseSkill = player.BigGuns;
                    break;
                case GoverningSkill.ENERGYWEAPONS:
                    BaseSkill = player.EnergyWeapons;
                    break;
                case GoverningSkill.MELEE:
                    BaseSkill = player.Melee;
                    break;
                case GoverningSkill.UNARMED:
                    BaseSkill = player.Unarmed;
                    break;
                case GoverningSkill.THROWN:
                    BaseSkill = player.Thrown;
                    break;
                default:
                    BaseSkill = 0;
                    break;
            }
            current.ActionPoints -= we.APCost;
            int HitChance = BaseSkill + (((we.RangeFeet + (player.Perception / 2)) - target.DistanceFeetRounded) * 3) - (target.Target.AC + this.LightPenalty);
            int result = Random.Next(0, 101);

            if(result > 0 && result <HitChance)
            {
                int damage = 0;

                for(int i = 0; i < we.NumberOfDice; i++)
                {
                    damage += Random.Next(1, we.WeaponDice + 1);
                }
                damage += we.WeaponModifier;
                resText = $"{resText} hitting for {damage}";
                target.Target.CurrentHp -= damage;
            }
            else
            {
                resText = $"{resText} and misses";
            }


            //if (targetIsPlayer)
            //{

            //    int index = PlayerTeam.IndexOf(PlayerTeam.First(e => e.ID == target.Target.ID));
            //    PlayerTeam.RemoveAt(index);
            //    if (target.Target.CurrentHp > 0)
            //        PlayerTeam.Insert(index, target.Target);
            //    else
            //        resText = $"{resText} and killing them";
            //}
            //else
            //{
            //    int index = EnemyTeam.IndexOf(EnemyTeam.First(e => e.ID == target.Target.ID));
            //    EnemyTeam.RemoveAt(index);
            //    if (target.Target.CurrentHp > 0)
            //        EnemyTeam.Insert(index, target.Target);
            //    else
            //        resText = $"{resText} and killing them";
            //}
            if (target.Target.CurrentHp <= 0)
            {
                resText = $"{resText} and kills them";

                int turnIndex = TurnOrder.IndexOf(TurnOrder.First(e => e.ID == target.Target.ID));
                TurnOrder.RemoveAt(turnIndex);

                if (targetIsPlayer)
                {
                    int index = PlayerTeam.IndexOf(PlayerTeam.First(e => e.ID == target.Target.ID));
                    PlayerTeam.RemoveAt(index);
                    PlayerDeaths.Add(new Result($"{target.Target.Name} was killed by {current.Name}", DateTime.Now));

                }
                else
                {
                    int index = EnemyTeam.IndexOf(EnemyTeam.First(e => e.ID == target.Target.ID));
                    EnemyTeam.RemoveAt(index);
                    EnemyDeaths.Add(new Result($"{target.Target.Name} was killed by {current.Name}", DateTime.Now));
                }
                currentTarget = null;
                SetComparisions();
            }
            this.Results.Add(new Result(resText, DateTime.Now));
        }

        public void MakeBeastAttack(Beast player, Attack we, BattleObjectComparison target)
        {

            bool targetIsPlayer = this.PlayerTeam.Any(e => e.ID == target.Target.ID);
            string resText = $"{current.Name} attacks {target.Target.Name} with {we.Name}";
            int BaseSkill = we.HitChance;
            current.ActionPoints -= we.APCost;
            int HitChance = BaseSkill + (((we.RangeFeet + (player.Perception / 2)) - target.DistanceFeetRounded) * 3) - (target.Target.AC + this.LightPenalty);
            int result = Random.Next(0, 101);

            if (result > 0 && result < HitChance)
            {
                int damage = 0;

                for (int i = 0; i < we.NumberOfDice; i++)
                {
                    damage += Random.Next(1, we.WeaponDice + 1);
                }
                damage += we.WeaponModifier;
                resText = $"{resText} hitting for {damage}";
                target.Target.CurrentHp -= damage;
            }
            else
            {
                resText = $"{resText} and misses";
            }

            if (target.Target.CurrentHp <= 0)
            {
                resText = $"{resText} and kills them";

                int turnIndex = TurnOrder.IndexOf(TurnOrder.First(e => e.ID == target.Target.ID));
                TurnOrder.RemoveAt(turnIndex);

                if (targetIsPlayer)
                {
                    int index = PlayerTeam.IndexOf(PlayerTeam.First(e => e.ID == target.Target.ID));
                    PlayerTeam.RemoveAt(index);
                    PlayerDeaths.Add(new Result($"{target.Target.Name} was killed by {current.Name}", DateTime.Now));
                }
                else
                {
                    int index = EnemyTeam.IndexOf(EnemyTeam.First(e => e.ID == target.Target.ID));
                    EnemyTeam.RemoveAt(index);
                    EnemyDeaths.Add(new Result($"{target.Target.Name} was killed by {current.Name}", DateTime.Now));
                }
                currentTarget = null;

                SetComparisions();
            }
            this.Results.Add(new Result(resText, DateTime.Now));
            
            //this.Results.Add(resText);
            //SetComparisions();
        }

        public void RunEncounter()
        {
            while (PlayerTeam.Count > 0 && EnemyTeam.Count > 0)
            {
                ExecuteTurnOrder();
            }
            if (PlayerTeam.Count == 0)
            {
                this.Results.Add(new Result($"---------- Enemies Win ----------\n\r\n\r", DateTime.MaxValue));
            }
            else
            {
                this.Results.Add(new Result($"---------- Players Win ----------\n\r\n\r", DateTime.MaxValue));
            }
            this.Results.Add(new Result($"{PlayerDeaths.Count} Players Died", DateTime.MaxValue));
            foreach(var item in PlayerDeaths)
            {
                Result r = new Result(item.text,DateTime.MaxValue);
                this.Results.Add(r);
            }
        }

        public void ExecuteTurnOrder()
        {
            int ptCount = PlayerTeam.Count;
            int etCount = EnemyTeam.Count;
            foreach (var item in TurnOrder)
            {
                
                current = item;
                current.ActionPoints = current.MaxActionPoints;
                CurrentTakeAction();
                currentAttack = null;
                currentWeapon = null;
                currentTarget = null;
                if (ptCount != PlayerTeam.Count || etCount != EnemyTeam.Count)
                {
                    break;
                }
            }
               
        }

        public int GetRise(BattleObject one, BattleObject two)
        {
            return one.YCord - two.YCord;
        }

        public int GetRun(BattleObject one, BattleObject two)
        {
            return one.XCord = two.YCord;
        }
    }
}
