using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StaffRandomSelect
{
    /// <summary>
    /// Input.xaml 的交互逻辑
    /// </summary>
    public partial class Input : Page
    {
        private Timer timer;
        public Input()
        {
            InitializeComponent();
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text.Trim().Length <= 0)
            {
                return;
            }
            if (ComboBox.Text.Length <= 0)
            {
                SetTimer(SnackbarTwo);
                return;
            }
            //正常时
            ModifyList(NameTextBox.Text, StarBar.Value, (Career)Enum.Parse(typeof(Career), ComboBox.Text));
        }

        private void SetTimer(Snackbar snackbar)
        {
            if (timer == null)
            {
                snackbar.IsActive = true;
            }
            else if (timer != null)
            {
                SnackbarOne.IsActive = false;
                SnackbarTwo.IsActive = false;
                SnackbarThree.IsActive = false;
                snackbar.IsActive = true;
                timer.Stop();
                timer = null;
            }
            timer = new Timer
            {
                Interval = 2000
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(MyEventHandler);
            timer.AutoReset = false;
            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private void ModifyList(string name, int star, Career career)
        {
            HashSet<string> nameSet = App.GetNameSet(); //获得名字集合
            if (nameSet.Contains(name))
            {
                SetTimer(SnackbarOne);
                return;
            }
            //列表中没有重复名
            App.staffLists.Add(new Staff { Name = name, Star = star, Career = career, IsSelected = true });
            SetTimer(SnackbarThree);
        }

        private void MyEventHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                SnackbarOne.IsActive = false;
                SnackbarTwo.IsActive = false;
                SnackbarThree.IsActive = false;
            }));
        }

        private void BasicRatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {

        }
    }
}
