﻿using FalloutPnpCalcTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FalloutPnpCalcTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Character> Characters { get; set; }
        public List<Beast> Beasts { get; set; }
        public List<Weapon> Weapons { get; set; }
        public List<Attack> Attacks { get; set; }
        public DateTime CharacterDate { get; set; }
        public DateTime BeastDate { get; set; }
        public DateTime WeaponDate { get; set; }
        public DateTime AttackDate { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }

        public MainWindow()
        {
            
            InitializeComponent();
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();

            this.Characters = App.Characters;
            this.CharacterDate = App.CharacterDate;

            this.Beasts = App.Beasts;
            this.BeastDate = App.BeastDate;

            this.Weapons = App.Weapons;
            this.WeaponDate = App.WeaponDate;

            this.Attacks = App.Attacks;
            this.AttackDate = App.AttackDate;

            SetCharacterView();
            SetBeastsView();
            SetAttacksView();
            SetWeaponsView();


        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(this.CharacterDate < App.CharacterDate)
            {
                this.Characters = App.Characters;
                this.CharacterDate = App.CharacterDate;
                SetCharacterView();
            }

            if(this.BeastDate < App.BeastDate)
            {
                this.Beasts = App.Beasts;
                this.BeastDate = App.BeastDate;
                SetBeastsView();
            }

            if(this.WeaponDate < App.WeaponDate)
            {
                this.Weapons = App.Weapons;
                this.WeaponDate = App.WeaponDate;
                SetWeaponsView();
                
            }

            if(this.AttackDate < App.AttackDate)
            {
                this.Attacks = App.Attacks;
                this.AttackDate = App.AttackDate;
                SetAttacksView();
            }
        }

        public void SetCharacterView()
        {
            StackPanel s = new StackPanel();
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            foreach (Character w in this.Characters.OrderBy(c => c.Name))
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label characterName = new Label();
                characterName.HorizontalAlignment = HorizontalAlignment.Left;
                characterName.FontSize = 15;
                characterName.VerticalAlignment = VerticalAlignment.Center;
                characterName.Content = w.Name;

                Grid.SetColumn(characterName, 0);
                g.Children.Add(characterName);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.CommandParameter = w.CharacterID;
                editButton.Click += EditCharacterButton_Click;
                Grid.SetColumn(editButton, 1);
                g.Children.Add(editButton);


                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.CharacterID;
                deleteButton.Click += DeleteCharacterButton_Click;

                Grid.SetColumn(deleteButton, 2);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }

            this.CharactersView.Content = s;
        }

        private void EditCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            Window w = new AddCharacter(id);
            w.Show();
        }

        private void DeleteCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            int i = App.Database.DeleteCharacter(id);
            this.Characters.Remove(this.Characters.FirstOrDefault(c => c.CharacterID == id));
            SetCharacterView();
        }

        public void SetBeastsView()
        {
            StackPanel s = new StackPanel();
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            foreach (Beast w in this.Beasts.OrderBy(b => b.Name))
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label beastName = new Label();
                beastName.HorizontalAlignment = HorizontalAlignment.Left;
                beastName.FontSize = 15;
                beastName.VerticalAlignment = VerticalAlignment.Center;
                beastName.Content = w.Name;

                Grid.SetColumn(beastName, 0);
                g.Children.Add(beastName);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.CommandParameter = w.ID;
                editButton.Click += EditBeastButton_Click;

                Grid.SetColumn(editButton, 1);
                g.Children.Add(editButton);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeleteBeastButton_Click;

                Grid.SetColumn(deleteButton, 2);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }

            this.BeastsView.Content = s;
        }

        private void DeleteBeastButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            int res = App.Database.DeleteBeast(id);
            this.Beasts.Remove(this.Beasts.FirstOrDefault(w => w.ID == id));
            SetBeastsView();
        }

        private void EditBeastButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            Window w = new AddBeast(id);
            w.Show();
        }

        public void SetWeaponsView()
        {
            StackPanel s = new StackPanel();
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            foreach (Weapon w in this.Weapons.OrderBy(b => b.WeaponName))
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label weaponName = new Label();
                weaponName.HorizontalAlignment = HorizontalAlignment.Left;
                weaponName.FontSize = 15;
                weaponName.VerticalAlignment = VerticalAlignment.Center;
                weaponName.Content = w.WeaponName;

                Grid.SetColumn(weaponName, 0);
                g.Children.Add(weaponName);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.CommandParameter = w.ID;
                editButton.Click += EditWeaponButton_Click;

                Grid.SetColumn(editButton, 1);
                g.Children.Add(editButton);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeleteWeaponButton_Click;

                Grid.SetColumn(deleteButton, 2);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }

            this.WeaponsView.Content = s;
        }

        private void DeleteWeaponButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            int res = App.Database.DeleteWeapon(id);
            this.Weapons.Remove(this.Weapons.FirstOrDefault(w => w.ID == id));
            SetWeaponsView();
        }

        private void EditWeaponButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            Window w = new AddWeapon(id);
            w.Show();
        }

        public void SetAttacksView()
        {
            StackPanel s = new StackPanel();
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            foreach (Attack w in this.Attacks.OrderBy(b => b.Name))
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4,GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label attackName = new Label();
                attackName.HorizontalAlignment = HorizontalAlignment.Left;
                attackName.FontSize = 15;
                attackName.VerticalAlignment = VerticalAlignment.Center;
                attackName.Content = w.Name;

                Grid.SetColumn(attackName, 0);
                g.Children.Add(attackName);

                Button editButton = new Button();
                editButton.Content = "Edit";
                editButton.CommandParameter = w.ID;
                editButton.Click += EditAttackButton_Click;

                Grid.SetColumn(editButton, 1);
                g.Children.Add(editButton);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeleteAttackButton_Click;

                Grid.SetColumn(deleteButton, 2);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }

            this.AttacksView.Content = s;
        }

        private void DeleteAttackButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            int res = App.Database.DeleteAttack(id);
            this.Attacks.Remove(this.Attacks.FirstOrDefault(w => w.ID == id));
            SetAttacksView();
        }

        private void EditAttackButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;
            Window w = new AddAttack(id);
            w.Show();
        }

        private void AddWeaponButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new AddWeapon();
            w.Show();
        }

        private void AddBeastButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new AddBeast();
            w.Show();
        }

        private void AddCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new AddCharacter();
            w.Show();
        }

        private void AddAttackButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new AddAttack();
            w.Show();
        }

        private void OpenCalcButton_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Calculator();
            w.Show();
        }

        private void ParseDataCSV_Click(object sender, RoutedEventArgs e)
        {
            ReadCSV("data");
        }

        public void ReadCSV(string fileName)
        {
            // We change file extension here to make sure it's a .csv file.
            // TODO: Error checking.
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".csv"));

            // lines.Select allows me to project each line as a Person. 
            // This will give me an IEnumerable<Person> back.
           foreach(string line in lines)
           {
                string[] data = line.Split(',');

                int type = 0;
                if(int.TryParse(data[0],out type))
                {
                    switch(type)
                    {
                        case 0:
                            AddCharacter(data);
                            break;
                        case 1:
                            AddWeapon(data);
                            break;
                        case 2:
                            AddPlayerWeapon(data);
                            break;
                        case 3:
                            AddBeast(data);
                            break;
                        case 4:
                            AddAttack(data);
                            break;
                        case 5:
                            AddBeastAttack(data);
                            break;

                    }
                }
                else
                {
                    continue;
                }
            }
        }

        public void AddCharacter(string[] data)
        {
            Guid cid = Guid.Empty;
            int Prcp = 0;
            int Ac = 0;
            int smg = 0;
            int bg = 0;
            int ew = 0;
            int me = 0;
            int ua = 0;
            int thr = 0;

            Character c = new Character();

            if(Guid.TryParse(data[1],out cid))
            {
                c.CharacterID = cid;
            }
            else
            {
                c.CharacterID = Guid.NewGuid();
            }

            if (int.TryParse(data[2], out Prcp))
            {
                c.Perception = Prcp;
            }
            else
            {
                c.Perception = Prcp;
            }


            if (int.TryParse(data[3], out Ac))
            {
                c.ArmorClass = Ac;
            }
            else
            {
                c.ArmorClass = Ac;
            }

            if (int.TryParse(data[4], out smg))
            {
                c.SmallGuns = smg;
            }
            else
            {
                c.SmallGuns = smg;
            }

            if (int.TryParse(data[5], out bg))
            {
                c.BigGuns = bg;
            }
            else
            {
                c.BigGuns = bg;
            }

            if (int.TryParse(data[6], out ew))
            {
                c.EnergyWeapons = ew;
            }
            else
            {
                c.EnergyWeapons = ew;
            }

            if (int.TryParse(data[7], out me))
            {
                c.Melee = me;
            }
            else
            {
                c.Melee = me;
            }

            if (int.TryParse(data[8], out ua))
            {
                c.Unarmed = ua;
            }
            else
            {
                c.Unarmed = ua;
            }

            if (int.TryParse(data[9], out thr))
            {
                c.Thrown = thr;
            }
            else
            {
                c.Thrown = thr;
            }

            c.Name = data[10];

            App.Database.SaveCharacter(c);
        }

        public void AddWeapon(string[] data)
        {
            Guid id = Guid.Empty;
            int dice = 0, numDice = 0, Range = 0, Modifier = 0, skill;
            Weapon w = new Weapon();

            if(Guid.TryParse(data[1],out id))
            {
                w.ID = id;
            }
            else
            {
                w.ID = Guid.NewGuid();
            }

            if (int.TryParse(data[2], out dice))
            {
                w.WeaponDice = dice;
            }
            else
            {
                w.WeaponDice = dice;
            }

            if (int.TryParse(data[3], out numDice))
            {
                w.NumberOfDice = numDice;
            }
            else
            {
                w.NumberOfDice = numDice;
            }

            if (int.TryParse(data[4], out Range))
            {
                w.RangeFeet = Range;
            }
            else
            {
                w.RangeFeet = Range;
            }

            if (int.TryParse(data[5], out Modifier))
            {
                w.WeaponModifier = Modifier;
            }
            else
            {
                w.WeaponModifier = Modifier;
            }

            if(int.TryParse(data[6],out skill))
            {
                w.skill = (GoverningSkill)skill;
            }
            else
            {
                w.skill = (GoverningSkill)skill;
            }

            w.WeaponName = data[7];

            App.Database.SaveWeapon(w);
        }

        public void AddPlayerWeapon(string [] data)
        {
            Guid id = Guid.Empty, cid = Guid.Empty, wid = Guid.Empty;

            PlayerWeapon pw = new PlayerWeapon();

            if(Guid.TryParse(data[1],out id))
            {
                pw.ID = id;
            }
            else
            {
                pw.ID = Guid.NewGuid();
            }

            if(Guid.TryParse(data[2], out cid) && Guid.TryParse(data[3], out wid))
            {
                pw.CharacterID = cid;
                pw.WeaponID = wid;

                App.Database.SavePlayerWeapon(pw);
            }
        }

        public void AddBeast(string [] data)
        {
            Guid id = Guid.Empty;
            int perception = 0, ac = 0;

            Beast b = new Beast();

            if(Guid.TryParse(data[1],out id))
            {
                b.ID = id;
            }
            else
            {
                b.ID = Guid.NewGuid();
            }

            if (int.TryParse(data[2], out perception))
            {
                b.Perception = perception;
            }
            else
            {
                b.Perception = perception;
            }

            if (int.TryParse(data[3], out ac))
            {
                b.ArmorClass = ac;
            }
            else
            {
                b.ArmorClass = ac;

            }

            b.Name = data[4];

            App.Database.SaveBeast(b);
        }

        public void AddAttack(string [] data)
        {
            Guid id = Guid.Empty;
            int dice = 0, numDice = 0, hc = 0, Modifier = 0;
            Attack a = new Attack();

            if(Guid.TryParse(data[1],out id))
            {
                a.ID = id;
            }
            else
            {
                a.ID = Guid.NewGuid();
            }

            if (int.TryParse(data[2], out dice))
            {
                a.WeaponDice = dice;
            }
            else
            {
                a.WeaponDice = dice;
            }

            if (int.TryParse(data[3], out numDice))
            {
                a.NumberOfDice = numDice;
            }
            else
            {
                a.NumberOfDice = numDice;

            }

            if (int.TryParse(data[4], out hc))
            {
                a.HitChance = hc;
            }
            else
            {
                a.HitChance = hc;
            }

            if (int.TryParse(data[5], out Modifier))
            {
                a.WeaponModifier = Modifier;
            }
            else
            {
                a.WeaponModifier = Modifier;

            }
            a.Name = data[6];

            App.Database.SaveAttack(a);
        }

        public void AddBeastAttack(string[] data)
        {
            Guid id = Guid.Empty, cid = Guid.Empty, wid = Guid.Empty;

            BeastAttack pw = new BeastAttack();

            if (Guid.TryParse(data[1], out id))
            {
                pw.ID = id;
            }
            else
            {
                pw.ID = Guid.NewGuid();
            }

            if (Guid.TryParse(data[2], out cid) && Guid.TryParse(data[3], out wid))
            {
                pw.BeastID = cid;
                pw.AttackID = wid;

                App.Database.SaveBeastAttack(pw);
            }
        }
    }

}
