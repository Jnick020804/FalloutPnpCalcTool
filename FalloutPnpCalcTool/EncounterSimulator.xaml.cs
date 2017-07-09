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
    /// Interaction logic for EncounterSimulator.xaml
    /// </summary>
    public partial class EncounterSimulator : Window
    {
        public List<CompositeBeing> PlayerTeam { get; set; }
        public List<CompositeBeing> EnemyTeam { get; set; }
        public DateTime BeingsDate { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }
        public EncounterSimulator()
        {
            PlayerTeam = new List<CompositeBeing>();
            EnemyTeam = new List<CompositeBeing>();
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();
            InitializeComponent();
            this.PlayerTeamDropdown.ItemsSource = App.Beings;
            this.EnemyTeamDropdown.ItemsSource = App.Beings;
            this.PlayerTeamDropdown.DisplayMemberPath = "Name";
            this.EnemyTeamDropdown.DisplayMemberPath = "Name";
            this.BeingsDate = App.BeingDate;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(this.BeingsDate < App.BeingDate)
            {
                this.PlayerTeamDropdown.ItemsSource = App.Beings;
                this.EnemyTeamDropdown.ItemsSource = App.Beings;
                this.PlayerTeamDropdown.DisplayMemberPath = "Name";
                this.EnemyTeamDropdown.DisplayMemberPath = "Name";
                this.BeingsDate = App.BeingDate;
            }
        }

        private void SetPlayerList()
        {
            StackPanel s = new StackPanel();
            foreach (CompositeBeing w in PlayerTeam)
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                Label weaponName = new Label();
                weaponName.HorizontalAlignment = HorizontalAlignment.Center;
                weaponName.VerticalAlignment = VerticalAlignment.Center;
                weaponName.Content = w.Name;

                Grid.SetColumn(weaponName, 0);
                g.Children.Add(weaponName);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeletePlayerButton_Click;

                Grid.SetColumn(deleteButton, 1);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }
            this.PlayerList.Content = s;
        }

        private void SetEnemyList()
        {
            StackPanel s = new StackPanel();
            foreach (CompositeBeing w in EnemyTeam)
            {
                Grid g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                Label weaponName = new Label();
                weaponName.HorizontalAlignment = HorizontalAlignment.Center;
                weaponName.VerticalAlignment = VerticalAlignment.Center;
                weaponName.Content = w.Name;

                Grid.SetColumn(weaponName, 0);
                g.Children.Add(weaponName);

                Button deleteButton = new Button();
                deleteButton.Content = "Delete";
                deleteButton.CommandParameter = w.ID;
                deleteButton.Click += DeleteEnemyButton_Click;

                Grid.SetColumn(deleteButton, 1);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }
            this.EnemyList.Content = s;
        }

        private void DeleteEnemyButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;

            try
            {
                EnemyTeam.Remove(EnemyTeam.First(pt => pt.ID == id));
                SetEnemyList();
            }
            catch (Exception ex)
            {

            }
        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            CompositeBeing cb = (CompositeBeing)this.PlayerTeamDropdown.SelectedItem;
            this.PlayerTeam.Add(cb);
            SetPlayerList();

        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Guid id = (Guid)b.CommandParameter;

            try
            {
                PlayerTeam.Remove(PlayerTeam.First(pt => pt.ID == id));
                SetPlayerList();
            }
            catch(Exception ex)
            {

            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            //set players on a 25 x 25 grid
            List<BattleObject> Players = new List<BattleObject>();
            List<BattleObject> Enemies = new List<BattleObject>();

            int lp = 10;
            int.TryParse(this.LPBox.Text, out lp);

            Random r = new Random();
            int x = 0;
            int y = 0;
            foreach(CompositeBeing item in this.PlayerTeam)
            {
                BattleObject ob = new BattleObject();
                
                if(item.Type == typeof(Character))
                {
                    Character ch = (Character)item.Object;
                    ob.AC = ch.ArmorClass;
                    ob.ActionPoints = ch.ActionPoints;
                    ob.Comparisons = new List<BattleObjectComparison>();
                    ob.CurrentHp = ch.MaxHitPoints;
                    ob.ID = Guid.NewGuid();
                    ob.MaxActionPoints = ch.ActionPoints;
                    ob.Name = ch.Name;
                    ob.Object = ch;
                    ob.Sequence = ch.Sequence;
                    ob.Type = item.Type;
                    ob.XCord = x;
                    ob.YCord = y;
                    Players.Add(ob);

                }
                else
                {
                    Beast ch = (Beast)item.Object;
                    ob.AC = ch.ArmorClass;
                    ob.ActionPoints = ch.ActionPoints;
                    ob.Comparisons = new List<BattleObjectComparison>();
                    ob.CurrentHp = ch.MaxHitPoints;
                    ob.ID = Guid.NewGuid();
                    ob.MaxActionPoints = ch.ActionPoints;
                    ob.Name = ch.Name;
                    ob.Object = ch;
                    ob.Sequence = ch.Sequence;
                    ob.Type = item.Type;
                    ob.XCord = x;
                    ob.YCord = y;
                    Players.Add(ob);

                }
                if (x >= 5)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    x++;
                }
            }

            foreach (CompositeBeing item in this.EnemyTeam)
            {
                BattleObject ob = new BattleObject();

                if (item.Type == typeof(Character))
                {
                    Character ch = (Character)item.Object;
                    ob.AC = ch.ArmorClass;
                    ob.ActionPoints = ch.ActionPoints;
                    ob.Comparisons = new List<BattleObjectComparison>();
                    ob.CurrentHp = ch.MaxHitPoints;
                    ob.ID = Guid.NewGuid();
                    ob.MaxActionPoints = ch.ActionPoints;
                    ob.Name = ch.Name;
                    ob.Object = ch;
                    ob.Sequence = r.Next(1,ch.Sequence+1);
                    ob.Type = item.Type;
                    ob.XCord = r.Next(0,26);
                    ob.YCord = r.Next(12,26);
                    Enemies.Add(ob);

                }
                else
                {
                    Beast ch = (Beast)item.Object;
                    ob.AC = ch.ArmorClass;
                    ob.ActionPoints = ch.ActionPoints;
                    ob.Comparisons = new List<BattleObjectComparison>();
                    ob.CurrentHp = ch.MaxHitPoints;
                    ob.ID = Guid.NewGuid();
                    ob.MaxActionPoints = ch.ActionPoints;
                    ob.Name = ch.Name;
                    ob.Object = ch;
                    ob.Sequence = r.Next(1, ch.Sequence + 1);
                    ob.Type = item.Type;
                    ob.XCord = r.Next(0, 26);
                    ob.YCord = r.Next(12, 26);
                    Enemies.Add(ob);
                }
            }

            BattleSimulationEngine bse = new BattleSimulationEngine(Players, Enemies, lp);
            bse.RunEncounter();

            ResultsList.ItemsSource = bse.Results.OrderBy(rl => rl.timeStamp);
            ResultsList.DisplayMemberPath = "ResultString";
        }

        private void AddEnemy_Click(object sender, RoutedEventArgs e)
        {
            CompositeBeing cb = (CompositeBeing)this.EnemyTeamDropdown.SelectedItem;
            this.EnemyTeam.Add(cb);
            SetEnemyList();
        }
    }
}
