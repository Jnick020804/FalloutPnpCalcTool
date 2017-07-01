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
    /// Interaction logic for AddBeast.xaml
    /// </summary>
    public partial class AddBeast : Window
    {
        public int Perception { get; set; }
        public int ArmorClass { get; set; }
        public string BName { get; set; }
        public Beast Beast { get; set; }
        public List<Attack> Attacks { get; set; }
        public DateTime AttackDate { get; set; }
        public DispatcherTimer DispatcherTimer { get; set; }

        public AddBeast(Guid? id = null)
        {
            InitializeComponent();
            DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();
            if (id.HasValue)
            {
                Beast b = new Beast();
                Task t1 = Task.Run(async () => { b = await App.Database.GetBeastAsync(id.Value); });
                t1.Wait();
                List<Attack> pws = new List<Attack>();
                Task t2 = Task.Run(async () => { pws = await App.Database.GetAttacksForBeastAsync(id.Value); });
                t2.Wait();
                this.Beast = b;
                this.Beast.Attacks = pws;
                SetValuesFromBeast(this.Beast);
                SetUIFromValues();

            }
            else
            {
                Beast b = new Beast();
                this.Beast = b;
            }

            this.Attacks = App.Attacks;
            this.AttackDate = App.AttackDate;


            this.WeaponDropdown.ItemsSource = this.Attacks;
            this.WeaponDropdown.DisplayMemberPath = "Name";
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(this.AttackDate < App.AttackDate)
            {
                this.Attacks = App.Attacks;
                this.AttackDate = App.AttackDate;


                this.WeaponDropdown.ItemsSource = this.Attacks;
                this.WeaponDropdown.DisplayMemberPath = "Name";
            }
        }

        private void SetUIFromValues()
        {
            this.PerceptionBox.Text = this.Perception.ToString();
            this.NameBox.Text = this.BName;
            this.ArmorClassBox.Text = this.ArmorClass.ToString();

            StackPanel s = new StackPanel();
            foreach (Attack w in Beast.Attacks)
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
                deleteButton.Click += DeleteButton_Click;

                Grid.SetColumn(deleteButton, 1);
                g.Children.Add(deleteButton);

                s.Children.Add(g);
            }
            this.WeaponsList.Content = s;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            Guid id = (Guid)s.CommandParameter;
            int res = App.Database.DeleteBeastAttack(id);
            Beast.Attacks.Remove(Beast.Attacks.FirstOrDefault(w => w.ID == id));
            SetUIFromValues();
        }

        private void SetValuesFromBeast(Beast beast)
        {
            this.Perception = beast.Perception;
            this.BName = beast.Name;
            this.ArmorClass = beast.ArmorClass;
        }

        private void SetValuesFromUI()
        {
            int perception = 0, ac = 0;

            if(int.TryParse(this.PerceptionBox.Text,out perception))
            {
                this.Perception = perception;
            }

            if(int.TryParse(this.ArmorClassBox.Text,out ac))
            {
                this.ArmorClass = ac;
            }

            this.BName = this.NameBox.Text;
        }

        private void SetBeastFromValues()
        {
            this.Beast.Perception = this.Perception;
            this.Beast.ArmorClass = this.ArmorClass;
            this.Beast.Name = this.BName;
        }

        private void AddWeapon_Click(object sender, RoutedEventArgs e)
        {
            Attack w = (Attack)this.WeaponDropdown.SelectedItem;
            if (this.Beast.ID != Guid.Empty)
            {
                BeastAttack pw = new BeastAttack();
                pw.ID = Guid.NewGuid();
                pw.BeastID = Beast.ID;
                pw.AttackID = w.ID;
                int res = App.Database.SaveBeastAttack(pw);
            }
            else
            {
                SaveBeast();
                BeastAttack pw = new BeastAttack();
                pw.ID = Guid.NewGuid();
                pw.BeastID = Beast.ID;
                pw.AttackID = w.ID;
                int res = App.Database.SaveBeastAttack(pw);
            }

            Beast.Attacks.Add(w);
            SetValuesFromBeast(this.Beast);

            SetUIFromValues();
        }

        public void SaveBeast()
        {
            SetValuesFromUI();
            SetBeastFromValues();
            if (this.Beast.ID == Guid.Empty)
            {
                this.Beast.ID = Guid.NewGuid();
            }
            int res = App.Database.SaveBeast(this.Beast);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveBeast();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
