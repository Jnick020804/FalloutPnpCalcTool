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
    }

}
