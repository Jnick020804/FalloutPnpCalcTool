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
    /// Interaction logic for AddWeapon.xaml
    /// </summary>
    public partial class AddWeapon : Window
    {
        public Guid ID { get; set; }
        public int WeaponDice { get; set; }
        public int NumberOfDice { get; set; }
        public int RangeFeet { get; set; }
        public int WeaponModifier { get; set; }
        public string WeaponName { get; set; }
        public GoverningSkill skill { get; set; }
        public Weapon Weapon { get; set; }
        public bool Initializing { get; set; }

        public Dictionary<int,string> GovSkills { get; set; }

        public AddWeapon(Guid? id = null)
        {
            InitializeComponent();
            GovSkills = new Dictionary<int, string>();

            for (int i = 0; i < 6; i++)
            {
                GovSkills.Add(i, ((Models.GoverningSkill)i).ToString());
            }

            this.GoverningSkill.ItemsSource = GovSkills;
            this.GoverningSkill.DisplayMemberPath = "Value";
            this.GoverningSkill.SelectedValuePath = "Key";
            if (id.HasValue)
            {
                Weapon w = new Weapon();
                Task t3 = Task.Run(async () => { w = await App.Database.GetWeaponAsync(id.Value); });
                 t3.Wait();
                this.Weapon = w;
                SetValuesFromWeapon(this.Weapon);
                SetUIFromValues();
                SetSampleRollsList();
            }
            else
            {
                Weapon w = new Weapon();
                this.Weapon = w;
            }

          
        }

        public void SetValuesFromWeapon(Weapon w)
        {
            this.ID = w.ID;
            this.WeaponDice = w.WeaponDice;
            this.NumberOfDice = w.NumberOfDice;
            this.RangeFeet = w.RangeFeet;
            this.WeaponModifier = w.WeaponModifier;
            this.WeaponName = w.WeaponName;
            this.skill = w.skill;
        }

        public void SetUIFromValues()
        {
            this.Initializing = true;
            this.DiceBox.Text = this.WeaponDice.ToString();
            this.NumDiceBox.Text = this.NumberOfDice.ToString();
            this.RangeBox.Text = this.RangeFeet.ToString();
            this.ModifierBox.Text = this.WeaponModifier.ToString();
            this.NameBox.Text = this.WeaponName;
            this.GoverningSkill.SelectedValue = (int)this.skill;
            this.Initializing = false;
        }

        public void SetValuesFromUI()
        {
            int dice = 0, numDice = 0, Range = 0, Modifier = 0;

            if(int.TryParse(this.DiceBox.Text,out dice))
            {
                this.WeaponDice = dice;
            }

            if(int.TryParse(this.NumDiceBox.Text, out numDice))
            {
                this.NumberOfDice = numDice;
            }

            if(int.TryParse(this.RangeBox.Text, out Range))
            {
                this.RangeFeet = Range;
            }

            if(int.TryParse(this.ModifierBox.Text, out Modifier))
            {
                this.WeaponModifier = Modifier;
            }

            if(this.GoverningSkill.SelectedItem != null)
            this.skill = (GoverningSkill)((KeyValuePair<int,string>)this.GoverningSkill.SelectedItem).Key;
            this.WeaponName= this.NameBox.Text;
        }

        public void SetWeaponFromValues()
        {
            this.Weapon.NumberOfDice = this.NumberOfDice;
            this.Weapon.RangeFeet = this.RangeFeet;
            this.Weapon.skill = this.skill;
            this.Weapon.WeaponDice = this.WeaponDice;
            this.Weapon.WeaponModifier = this.WeaponModifier;
            this.Weapon.WeaponName = this.WeaponName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetValuesFromUI();
            SetWeaponFromValues();
            if(this.Weapon.ID == Guid.Empty)
            {
                this.Weapon.ID = Guid.NewGuid();
            }
            int res = App.Database.SaveWeapon(this.Weapon);

        }

        public void SetSampleRollsList()
        {
            SetValuesFromUI();
            SetWeaponFromValues();
            Random r = new Random((int)DateTime.Now.Ticks);
            List<string> valuesList = new List<string>();
            for (int i = 0; i < 10; i++ )
            {
                string srString = getSampleRoll(r);
                string itemString = $"{i+1}.) {srString}";
                valuesList.Add(itemString);
                
            }

            this.SampleRollsList.ItemsSource = valuesList;
        }

        public string getSampleRoll(Random r = null)
        {
            string MDString = (this.Weapon.skill == Models.GoverningSkill.MELEE || this.Weapon.skill == Models.GoverningSkill.UNARMED) ? " + MD" :"";
            if(r == null)
            r = new Random((int)DateTime.Now.Ticks);
            int total = 0;
            for(int i = 0; i < this.Weapon.NumberOfDice; i++)
            {
                total += r.Next(1, this.Weapon.WeaponDice + 1);
            }

            total += this.Weapon.WeaponModifier;
     
            return $"Rolling {this.Weapon.NumberOfDice}d{this.Weapon.WeaponDice} + {this.Weapon.WeaponModifier} for a total of {total}{MDString} points of damage";

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

            this.DiceBox.Text = "";
            this.WeaponDice = 0;
            this.NumDiceBox.Text = "";
            this.NumberOfDice = 0;
            this.RangeBox.Text = "";
            this.RangeFeet = 0;
            this.ModifierBox.Text = "";
            this.WeaponModifier = 0;
            this.NameBox.Text = "";
            this.WeaponName = "";

        }

        private void DiceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!this.Initializing)
            SetSampleRollsList();
        }

        private void ReRoll_Click(object sender, RoutedEventArgs e)
        {
            SetSampleRollsList();
        }
    }
}
