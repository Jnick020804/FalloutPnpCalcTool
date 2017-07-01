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
                this.Beast = b;
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

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetValuesFromUI();
            SetBeastFromValues();
            if (this.Beast.ID == Guid.Empty)
            {
                this.Beast.ID = Guid.NewGuid();
            }
            int res = App.Database.SaveBeast(this.Beast);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
