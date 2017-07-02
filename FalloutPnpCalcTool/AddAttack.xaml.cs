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

namespace FalloutPnpCalcTool
{
    /// <summary>
    /// Interaction logic for AddAttack.xaml
    /// </summary>
    public partial class AddAttack : Window
    {
        public int NumberOfDice { get; set; }
        public int WeaponDice { get; set; }
        public int WeaponModifier { get; set; }
        public int HitChance { get; set; }
        public string AName { get; set; }
        public Attack Attack { get; set; }
        public bool Initializing { get; set; }

        public AddAttack(Guid? id = null)
        {
            InitializeComponent();
            if (id.HasValue)
            {
                Attack a = new Attack();
                Task t3 = Task.Run(async () => { a = await App.Database.GetAttackAsync(id.Value); });
                t3.Wait();
                this.Attack = a;
                SetValuesFromAttack(this.Attack);
                SetUIFromValues();
                SetSampleRollsList();
            }
            else
            {
                Attack a = new Attack();
                this.Attack = a;
            }
        }

        public void SetSampleRollsList()
        {
            SetValuesFromUI();
            SetAttackFromValues();
            Random r = new Random((int)DateTime.Now.Ticks);
            List<string> valuesList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                string srString = getSampleRoll(r);
                string itemString = $"{i + 1}.) {srString}";
                valuesList.Add(itemString);

            }

            this.SampleRollsList.ItemsSource = valuesList;
        }

        public string getSampleRoll(Random r = null)
        {
            if (r == null)
                r = new Random((int)DateTime.Now.Ticks);
            int total = 0;
            for (int i = 0; i < this.Attack.NumberOfDice; i++)
            {
                total += r.Next(1, this.Attack.WeaponDice + 1);
            }

            total += this.Attack.WeaponModifier;

            return $"Rolling {this.Attack.NumberOfDice}d{this.Attack.WeaponDice} + {this.Attack.WeaponModifier} for a total of {total} points of damage";

        }

        private void SetUIFromValues()
        {
            this.Initializing = true;
            this.NumDiceBox.Text = this.NumberOfDice.ToString();
            this.DiceBox.Text = this.WeaponDice.ToString();
            this.ModifierBox.Text = this.WeaponModifier.ToString();
            this.HitChanceBox.Text = this.HitChance.ToString();
            this.NameBox.Text = this.AName;
            this.Initializing = false;
        }

        private void SetValuesFromAttack(Attack attack)
        {
            this.NumberOfDice = attack.NumberOfDice;
            this.WeaponDice = attack.WeaponDice;
            this.WeaponModifier = attack.WeaponModifier;
            this.HitChance = attack.HitChance;
            this.AName = attack.Name;
        }

        public void SetValuesFromUI()
        {
            int dice = 0, numDice = 0, hc = 0, Modifier = 0;

            if (int.TryParse(this.DiceBox.Text, out dice))
            {
                this.WeaponDice = dice;
            }

            if (int.TryParse(this.NumDiceBox.Text, out numDice))
            {
                this.NumberOfDice = numDice;
            }

            if (int.TryParse(this.HitChanceBox.Text, out hc))
            {
                this.HitChance = hc;
            }

            if (int.TryParse(this.ModifierBox.Text, out Modifier))
            {
                this.WeaponModifier = Modifier;
            }
            this.AName = this.NameBox.Text;
        }

        public void SetAttackFromValues()
        {
            this.Attack.NumberOfDice = this.NumberOfDice;
            this.Attack.HitChance = this.HitChance;
            this.Attack.WeaponDice = this.WeaponDice;
            this.Attack.WeaponModifier = this.WeaponModifier;
            this.Attack.Name = this.AName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetValuesFromUI();
            SetAttackFromValues();
            if (this.Attack.ID == Guid.Empty)
            {
                this.Attack.ID = Guid.NewGuid();
            }
            int res = App.Database.SaveAttack(this.Attack);
        }

        private void DiceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.Initializing)
                SetSampleRollsList();
        }

        private void ReRoll_Click(object sender, RoutedEventArgs e)
        {
            SetSampleRollsList();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.NumDiceBox.Text = "";
            this.NumberOfDice = 0;
            this.DiceBox.Text = "";
            this.WeaponDice = 0;
            this.ModifierBox.Text = "";
            this.WeaponModifier = 0;
            this.HitChanceBox.Text = "";
            this.HitChance = 0;
            this.NameBox.Text = "";
            this.AName = "";
        }
    }
}
