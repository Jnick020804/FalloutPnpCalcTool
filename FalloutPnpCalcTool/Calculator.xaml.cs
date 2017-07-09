using FalloutPnpCalcTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FalloutPnpCalcTool
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    //public partial class Calculator : Window
    //{
    //    public Calculator()
    //    {
    //        InitializeComponent();
    //    }
    //}

    public partial class Calculator : Window
    {
        List<ToolInstance> Instances { get; set; }
        public Guid ID { get; set; }
        public int BaseSkill { get; set; }
        public int TargetAC { get; set; }
        public int WeaponRange { get; set; }
        public int Perception { get; set; }
        public int Distance { get; set; }
        public int HitChance { get; set; }
        public int RollToHit { get; set; }
        public int Damage { get; set; }
        public int Threshold { get; set; }
        public double Resistance { get; set; }
        public double Modifier { get; set; }
        public int Critical { get; set; }
        public int Light { get; set; }
        public int Target { get; set; }
        public string HitChanceText { get; set; }
        public string HitResultText { get; set; }
        public string DamageText { get; set; }
        public string CharName { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }
        public List<CompositeBeing> Beings { get; set; }
        public DateTime BeingsDate { get; set; }
        public Type AttackWithType { get; set; }
        public bool WeaponsChanging { get; set; }
        public Calculator()
        {
            Instances = new List<ToolInstance>();
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();
            this.HitChanceText = "0%";
            this.ID = Guid.NewGuid();
            InitializeComponent();
            Beings = new List<CompositeBeing>();
            BeingsDate = App.BeingDate;
            Beings = App.Beings;
            this.AttackerDropDown.ItemsSource = Beings;
            this.AttackerDropDown.DisplayMemberPath = "Name";
            this.DefenderDropDown.ItemsSource = Beings;
            this.DefenderDropDown.DisplayMemberPath = "Name";
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateValues();
            UpdateList();
            if (BeingsDate < App.BeingDate)
            {
                Beings = App.Beings;
                BeingsDate = App.BeingDate;
                this.AttackerDropDown.ItemsSource = Beings;
                this.DefenderDropDown.ItemsSource = Beings;

            }
        }

        private void UpdateUI(ToolInstance t)
        {
            this.BaseSkillBox.Text = t.BaseSkill.ToString();
            this.TargetACBox.Text = t.TargetAC.ToString();
            this.WeaponRangeBox.Text = t.WeaponRange.ToString();
            this.PerceptionBox.Text = t.Perception.ToString();
            this.DistanceBox.Text = t.Distance.ToString();
            this.HitChanceBox.Text = t.HitChance.ToString();
            this.RollToHitBox.Text = t.RollToHit.ToString();
            this.WeaponDamageBox.Text = t.Damage.ToString();
            this.DamageText = t.DamageText?.ToString() ?? "";
            this.ThresholdBox.Text = t.Threshold.ToString();
            this.DamageResistanceBox.Text = t.Resistance.ToString();
            this.ModifierBox.Text = t.Modifier.ToString();
            this.CriticalChanceBox.Text = t.Critical.ToString();
            this.LightBox.Text = t.Light.ToString();
            this.TargetBox.Text = t.Target.ToString();
            this.HitResult.Text = t.HitResultText;
            this.NameBox.Text = t?.Name ?? "NO NAME";
            this.ID = t.ID;
        }

        private void ClearUI()
        {
            this.BaseSkillBox.Text = "";
            this.TargetACBox.Text = "";
            this.WeaponRangeBox.Text = "";
            this.PerceptionBox.Text = "";
            this.DistanceBox.Text = "";
            this.HitChanceBox.Text = "";
            this.RollToHitBox.Text = "";
            this.WeaponDamageBox.Text = "";
            this.DamageText = "";
            this.ThresholdBox.Text = "";
            this.DamageResistanceBox.Text = "";
            this.ModifierBox.Text = "";
            this.CriticalChanceBox.Text = "";
            this.LightBox.Text = "";
            this.TargetBox.Text = "";
            this.HitResult.Text = "";
            this.NameBox.Text = "";
        }

        private void UpdateList()
        {
            try
            {
                int index = this.Instances.IndexOf(this.Instances.FirstOrDefault(i => i.ID == this.ID));
                this.Instances.RemoveAt(index);
                this.Instances.Insert(index, this.ReturnInstance());
            }
            catch (Exception e)
            {

            }
        }

        private void UpdateValues()
        {
            this.CharName = !string.IsNullOrWhiteSpace(this.NameBox.Text) ? this.NameBox.Text : $"NO NAME {this.Instances.Count + 1}";

            int bs;
            if (int.TryParse(this.BaseSkillBox.Text, out bs))
            {
                this.BaseSkill = bs;
            }
            else
            {
                this.BaseSkill = 0;
            }

            int tac;
            if (int.TryParse(this.TargetACBox.Text, out tac))
            {
                this.TargetAC = tac;
            }
            else
            {
                this.TargetAC = 0;
            }

            int wr;
            if (int.TryParse(this.WeaponRangeBox.Text, out wr))
            {
                this.WeaponRange = wr;
            }
            else
            {
                this.WeaponRange = 0;
            }

            int p;
            if (int.TryParse(this.PerceptionBox.Text, out p))
            {
                this.Perception = p;
            }
            else
            {
                this.Perception = 0;
            }

            int d;
            if (int.TryParse(this.DistanceBox.Text, out d))
            {
                this.Distance = d;
            }
            else
            {
                this.Distance = 0;
            }

            int rth;
            if (int.TryParse(this.RollToHitBox.Text, out rth))
            {
                this.RollToHit = rth;
            }
            else
            {
                this.RollToHit = 0;
            }

            int dmg;
            if (int.TryParse(this.WeaponDamageBox.Text, out dmg))
            {
                this.Damage = dmg;
            }
            else
            {
                this.Damage = 0;
            }

            int thr;
            if (int.TryParse(this.ThresholdBox.Text, out thr))
            {
                this.Threshold = thr;
            }
            else
            {
                this.Threshold = 0;
            }

            double res;
            if (double.TryParse(this.DamageResistanceBox.Text, out res))
            {
                this.Resistance = res;
            }
            else
            {
                this.Resistance = 0;
            }

            double mdf;
            if (double.TryParse(this.ModifierBox.Text, out mdf))
            {
                this.Modifier = mdf;
            }
            else
            {
                this.Modifier = 0;
            }

            int crit;
            if (int.TryParse(this.CriticalChanceBox.Text, out crit))
            {
                this.Critical = crit;
            }
            else
            {
                this.Critical = 0;
            }

            int lght;
            if (int.TryParse(this.LightBox.Text, out lght))
            {
                this.Light = lght;
            }
            else
            {
                this.Light = 0;
            }

            int trgt;
            if (int.TryParse(this.TargetBox.Text, out trgt))
            {
                this.Target = trgt;
            }
            else
            {
                this.Target = 0;
            }

        }

        private void CalcHitChance_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            this.HitChance = this.BaseSkill + (((this.WeaponRange + (this.Perception / 2)) - this.Distance) * 3) - (this.TargetAC + this.Light + this.Target);
            if (this.HitChance < 0)
            {
                this.HitChance = 0;
            }
            this.HitChanceText = this.HitChance.ToString() + "%";
            this.HitChanceBox.Text = this.HitChanceText;
        }

        private void HitResultButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            bool hit = false;
            bool critical = false;
            if (this.HitChance >= this.RollToHit)
            {
                hit = true;
            }

            if (this.RollToHit <= this.Critical)
            {
                critical = true;
            }

            if (hit && critical)
            {
                this.HitResultText = "Critical Hit";
            }
            else if (hit && !critical)
            {
                this.HitResultText = "Hit";
            }
            else
            {
                double percentMiss = 0;

                if (this.HitChance != 0)
                {
                    percentMiss = (double)(this.RollToHit - this.HitChance) / (double)this.HitChance;
                    percentMiss = percentMiss * 100;
                }
                else
                {
                    percentMiss = 100;
                }

                if (percentMiss >= 97)
                {
                    this.HitResultText = "Critical Miss";
                }
                else
                {
                    this.HitResultText = "Miss";
                }
            }

            if (RollToHit == 0)
            {
                this.HitResultText = "Critical Hit";
            }

            this.HitResult.Text = this.HitResultText;
        }

        private void CalcDamage_Click(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            int d = (this.Damage - this.Threshold) - (int)Math.Floor((this.Damage - this.Threshold) * this.Resistance);
            if (this.Modifier != 0)
            {
                d = (int)Math.Round((double)d * Modifier);
            }
            this.DamageText = "Hit for " + d.ToString();
            this.DamageTextBox.Text = this.DamageText;
        }

        public void StoreInstance()
        {
            if (Instances.FirstOrDefault(i => i.ID == this.ID) == null)
            {
                ToolInstance temp = new ToolInstance();
                temp.BaseSkill = this.BaseSkill;
                temp.Critical = this.Critical;
                temp.Damage = this.Damage;
                temp.DamageText = this.DamageText;
                temp.Distance = this.Distance;
                temp.HitChance = this.HitChance;
                temp.HitChanceText = this.HitChanceText;
                temp.HitResultText = this.HitResultText;
                temp.Light = this.Light;
                temp.Modifier = this.Modifier;
                temp.Name = this.Name;
                temp.Perception = this.Perception;
                temp.Resistance = this.Resistance;
                temp.RollToHit = this.RollToHit;
                temp.Target = this.Target;
                temp.TargetAC = this.TargetAC;
                temp.Threshold = this.Threshold;
                temp.WeaponRange = this.WeaponRange;
                temp.Name = this.CharName;
                temp.ID = this.ID;

                this.Instances.Add(temp);
            }
        }

        public ToolInstance ReturnInstance()
        {
            ToolInstance temp = new ToolInstance();
            temp.BaseSkill = this.BaseSkill;
            temp.Critical = this.Critical;
            temp.Damage = this.Damage;
            temp.DamageText = this.DamageText;
            temp.Distance = this.Distance;
            temp.HitChance = this.HitChance;
            temp.HitChanceText = this.HitChanceText;
            temp.HitResultText = this.HitResultText;
            temp.Light = this.Light;
            temp.Modifier = this.Modifier;
            temp.Name = this.Name;
            temp.Perception = this.Perception;
            temp.Resistance = this.Resistance;
            temp.RollToHit = this.RollToHit;
            temp.Target = this.Target;
            temp.TargetAC = this.TargetAC;
            temp.Threshold = this.Threshold;
            temp.WeaponRange = this.WeaponRange;
            temp.Name = this.CharName;
            temp.ID = this.ID;

            return temp;
        }


        private void SwitchInstance(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            UpdateList();
            Button b = (Button)sender;
            int id = (int)b.CommandParameter;
            ToolInstance t = Instances.ElementAt(id);
            this.UpdateUI(t);


        }


        private void AttackerDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            CompositeBeing cb = (CompositeBeing)c.SelectedItem;


            if (cb.Type == typeof(Character))
            {
                AttackWithType = typeof(Weapon);
                Character ch = (Character)cb.Object;
                this.Perception = ch.Perception;
                this.WeaponRange = 0;
                this.Distance = 0;
                SetName();
                UpdateUI(ReturnInstance());
                List<Weapon> w = new List<Weapon>();
                Task t1 = Task.Run(async () => { w = await App.Database.GetWeaponsForCharacterAsync(ch.CharacterID); });
                t1.Wait();
                this.WeaponsChanging = true;
                this.AttackWithDropDown.ItemsSource = w;
                this.AttackWithDropDown.DisplayMemberPath = "WeaponName";
                this.WeaponsChanging = false;
            }
            else
            {
                AttackWithType = typeof(Attack);
                Beast b = (Beast)cb.Object;
                this.Perception = b.Perception;
                this.WeaponRange = 0;
                this.Distance = 0;
                SetName();
                UpdateUI(ReturnInstance());
                List<Attack> w = new List<Attack>();
                Task t1 = Task.Run(async () => { w = await App.Database.GetAttacksForBeastAsync(b.ID); });
                t1.Wait();
                this.WeaponsChanging = true;
                this.AttackWithDropDown.ItemsSource = w;
                this.AttackWithDropDown.DisplayMemberPath = "Name";
                this.WeaponsChanging = false;
            }
        }

        private void DefenderDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            CompositeBeing cb = (CompositeBeing)c.SelectedItem;

            if (cb.Type == typeof(Character))
            {
                Character ch = (Character)cb.Object;
                this.TargetAC = ch.ArmorClass;
                SetName();
                UpdateUI(ReturnInstance());
            }
            else
            {
                Beast b = (Beast)cb.Object;
                this.TargetAC = b.ArmorClass;
                SetName();
                UpdateUI(ReturnInstance());
            }
        }

        public void SetName()
        {
            CompositeBeing cb = (CompositeBeing)this.AttackerDropDown.SelectedItem;
            string AttackerName = "";
            if (cb != null)
            {
                if (cb.Type == typeof(Character))
                {
                    Character c = (Character)cb.Object;
                    AttackerName = c.Name;
                }
                else
                {
                    Beast b = (Beast)cb.Object;
                    AttackerName = b.Name;
                }
            }

            CompositeBeing cb2 = (CompositeBeing)this.DefenderDropDown.SelectedItem;
            string Defender = "";
            if (cb2 != null)
            {
                if (cb2.Type == typeof(Character))
                {
                    Character ch = (Character)cb2.Object;
                    Defender = ch.Name;
                }
                else
                {
                    Beast b = (Beast)cb2.Object;
                    Defender = b.Name;
                }
            }

            this.CharName = $"{AttackerName} VS {Defender}";




        }

        private void AttackWithDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.WeaponsChanging)
            {
                if (AttackWithType == typeof(Weapon))
                {
                    ComboBox c = (ComboBox)sender;
                    Weapon w = (Weapon)c.SelectedItem;

                    CompositeBeing cb = (CompositeBeing)this.AttackerDropDown.SelectedItem;
                    Character ch = (Character)cb.Object;

                    switch (w.skill)
                    {
                        case GoverningSkill.SMALLGUNS:
                            this.BaseSkill = ch.SmallGuns;
                            break;
                        case GoverningSkill.BIGGUNS:
                            this.BaseSkill = ch.BigGuns;
                            break;
                        case GoverningSkill.ENERGYWEAPONS:
                            this.BaseSkill = ch.EnergyWeapons;
                            break;
                        case GoverningSkill.MELEE:
                            this.BaseSkill = ch.Melee;
                            break;
                        case GoverningSkill.UNARMED:
                            this.BaseSkill = ch.Unarmed;
                            break;
                        case GoverningSkill.THROWN:
                            this.BaseSkill = ch.Thrown;
                            break;
                        default:
                            this.BaseSkill = 0;
                            break;
                    }

                    this.WeaponRange = w.RangeFeet;

                   
                }
                else
                {
                    ComboBox c = (ComboBox)sender;
                    Attack w = (Attack)c.SelectedItem;
                    this.BaseSkill = w.HitChance;


                }

                UpdateUI(ReturnInstance());
            }
        }
    }

    public class ToolInstance
    {
        public int BaseSkill { get; set; }
        public int TargetAC { get; set; }
        public int WeaponRange { get; set; }
        public int Perception { get; set; }
        public int Distance { get; set; }
        public int HitChance { get; set; }
        public int RollToHit { get; set; }
        public int Damage { get; set; }
        public int Threshold { get; set; }
        public double Resistance { get; set; }
        public double Modifier { get; set; }
        public int Critical { get; set; }
        public int Light { get; set; }
        public int Target { get; set; }
        public string HitChanceText { get; set; }
        public string HitResultText { get; set; }
        public string DamageText { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }
    }
}
