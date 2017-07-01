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
    /// Interaction logic for AddCharacter.xaml
    /// </summary>
    public partial class AddCharacter : Window
    {

        public Guid CharacterID { get; set; }
        public int Perception { get; set; }
        public int SmallGuns { get; set; }
        public int Melee { get; set; }
        public int Unarmed { get; set; }
        public int Thrown { get; set; }
        public int EnergyWeapons { get; set; }
        public int BigGuns { get; set; }
        public int ArmorClass { get; set; }
        public string CName { get; set; }
        public Character c { get; set; }
        public List<Weapon> Weapons { get; set; }
        public DateTime WeaponsDate { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }
        public AddCharacter(Guid? id = null)
        {
            InitializeComponent();
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();
            if (id.HasValue)
            {
                List<Weapon> pws = new List<Weapon>();
                Task t2 = Task.Run(async () => { pws = await App.Database.GetWeaponsForCharacterAsync(id.Value); });
                t2.Wait();
                c = App.Characters.FirstOrDefault(ch => ch.CharacterID == id.Value);
                c.Weapons = pws;
                SetValuesFromCharacter(c);
                SetFormFromValues();
            }
            else
            {
                Character c = new Character();
                c.Weapons = new List<Weapon>();
                this.c = c;
            }

            this.Weapons = App.Weapons;
            this.WeaponsDate = App.WeaponDate;
            this.WeaponDropdown.ItemsSource = this.Weapons;
            this.WeaponDropdown.DisplayMemberPath = "WeaponName";

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(App.WeaponDate > this.WeaponsDate)
            {
                this.Weapons = App.Weapons;
                this.WeaponsDate = App.WeaponDate;
                this.WeaponDropdown.ItemsSource = this.Weapons;
            }
        }

        public void SetValuesFromCharacter(Character c)
        {
            this.CharacterID = c.CharacterID;
            this.Perception = c.Perception;
            this.SmallGuns = c.SmallGuns;
            this.Melee = c.Melee;
            this.Unarmed = c.Unarmed;
            this.Thrown = c.Thrown;
            this.EnergyWeapons = c.EnergyWeapons;
            this.BigGuns = c.BigGuns;
            this.ArmorClass = c.ArmorClass;
            this.CName = c.Name;

        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            Guid id = (Guid)s.CommandParameter;
            int res = App.Database.DeletePlayerWeapon(id);
            c.Weapons.Remove(c.Weapons.FirstOrDefault(w => w.ID == id));
            SetFormFromValues();

        }

        public void SetCharacterValues()
        {
            this.c.CharacterID = this.CharacterID;
            this.c.Perception = this.Perception;
            this.c.SmallGuns = this.SmallGuns;
            this.c.Melee = this.Melee;
            this.c.Unarmed = this.Unarmed;
            this.c.Thrown = this.Thrown;
            this.c.EnergyWeapons = this.EnergyWeapons;
            this.c.BigGuns = this.BigGuns;
            this.c.ArmorClass = this.ArmorClass;
            this.c.Name = this.CName;
        }

        public void SetValuesFromForm()
        {
            int Prcp = 0;
            int Ac = 0;
            int smg = 0;
            int bg = 0;
            int ew = 0;
            int me = 0;
            int ua = 0;
            int thr = 0;
            this.CName = this.NameBox.Text;

            if(int.TryParse(this.PerceptionBox.Text,out Prcp))
            {
                this.Perception = Prcp;
            }

            if(int.TryParse(this.ArmorClassBox.Text,out Ac))
            {
                this.ArmorClass = Ac;
            }

            if (int.TryParse(this.SmallGunsBox.Text, out smg))
            {
                this.SmallGuns = smg;
            }

            if (int.TryParse(this.BigGunsBox.Text, out bg))
            {
                this.BigGuns = bg;
            }

            if (int.TryParse(this.EnergyWeaponsBox.Text, out ew))
            {
                this.EnergyWeapons = ew;
            }

            if (int.TryParse(this.MeleeBox.Text, out me))
            {
                this.Melee = me;
            }

            if (int.TryParse(this.UnarmedBox.Text, out ua))
            {
                this.Unarmed = ua;
            }

            if (int.TryParse(this.ThrownBox.Text, out thr))
            {
                this.Thrown = thr;
            }
        }

        public void SetFormFromValues()
        {
            this.PerceptionBox.Text = this.Perception.ToString();
            this.ArmorClassBox.Text = this.ArmorClass.ToString();
            this.SmallGunsBox.Text = this.SmallGuns.ToString();
            this.BigGunsBox.Text = this.BigGuns.ToString();
            this.EnergyWeaponsBox.Text = this.EnergyWeapons.ToString();
            this.UnarmedBox.Text = this.Unarmed.ToString();
            this.MeleeBox.Text = this.Melee.ToString();
            this.ThrownBox.Text = this.Thrown.ToString();
            this.NameBox.Text = this.CName;
            this.WeaponDropdown.ItemsSource = this.Weapons;

            StackPanel s = new StackPanel();
            foreach (Weapon w in c.Weapons)
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                Label weaponName = new Label();
                weaponName.HorizontalAlignment = HorizontalAlignment.Center;
                weaponName.VerticalAlignment = VerticalAlignment.Center;
                weaponName.Content = w.WeaponName;

                Grid.SetColumn(weaponName, 0);
                g.Children.Add(weaponName);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeleteButton_Click;

                Grid.SetColumn(deleteButton, 1);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }
            this.WeaponsList.Content = s;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCharacter();
        }

        private void SaveCharacter()
        {
            SetValuesFromForm();
            if (this.c.CharacterID != Guid.Empty)
            {
                c.ArmorClass = this.ArmorClass;
                c.BigGuns = this.BigGuns;
                c.EnergyWeapons = this.EnergyWeapons;
                c.Melee = this.Melee;
                c.Name = this.CName;
                c.Perception = this.Perception;
                c.SmallGuns = this.SmallGuns;
                c.Thrown = this.Thrown;
                c.Unarmed = this.Unarmed;

                int i = App.Database.SaveCharacter(c);
            }
            else
            {
                c.CharacterID = Guid.NewGuid();
                c.ArmorClass = this.ArmorClass;
                c.BigGuns = this.BigGuns;
                c.EnergyWeapons = this.EnergyWeapons;
                c.Melee = this.Melee;
                c.Name = this.CName;
                c.Perception = this.Perception;
                c.SmallGuns = this.SmallGuns;
                c.Thrown = this.Thrown;
                c.Unarmed = this.Unarmed;
                int i = App.Database.SaveCharacter(c);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddWeapon_Click(object sender, RoutedEventArgs e)
        {
            Weapon w = (Weapon)this.WeaponDropdown.SelectedItem;
            if(this.c.CharacterID != Guid.Empty)
            {
                PlayerWeapon pw = new PlayerWeapon();
                pw.ID = Guid.NewGuid();
                pw.CharacterID = c.CharacterID;
                pw.WeaponID = w.ID;
                int res = App.Database.SavePlayerWeapon(pw);
            }
            else
            {
                SaveCharacter();
                PlayerWeapon pw = new PlayerWeapon();
                pw.ID = Guid.NewGuid();
                pw.CharacterID = c.CharacterID;
                pw.WeaponID = w.ID;
                int res = App.Database.SavePlayerWeapon(pw);
            }

            c.Weapons.Add(w);
            SetValuesFromCharacter(c);

            SetFormFromValues();
        }
    }
}
