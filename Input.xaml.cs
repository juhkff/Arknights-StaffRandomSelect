using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class Input : ContentControl
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
            App.staffLists.Add(new Staff { Name = name, Star = star, Career = career, IsSelected = true, Level = new Level(int.Parse(EliteTextBox.Text), int.Parse(RankTextBox.Text)) });
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

        private void EliteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //只接受0、1的值和tab键、左右键
            //屏蔽非法按键
            bool result1 = (e.Key < Key.NumPad0
                || e.Key > Key.NumPad2) && (e.Key < Key.D0 || e.Key > Key.D2) && e.Key != Key.Back && e.Key != Key.Tab && e.Key != Key.Left && e.Key != Key.Right;  //输入只能是0、1、2和tab、退格
            if (result1)
            {
                e.Handled = true;
            }
            else if (EliteTextBox.Text != "" && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right)
            {
                e.Handled = true;
            }
        }

        private void RankTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool result1 = (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) && (e.Key < Key.D0 || e.Key > Key.D9);
            if (e.Key != Key.Back && e.Key != Key.Tab && e.Key != Key.Left && e.Key != Key.Right && (result1 || RankTextBox.Text.Length >= 2))   //输入只能是0、1、2和tab、退格、左右键，且长度不得超过2
            {
                e.Handled = true;
            }
            else if (!result1 && RankTextBox.Text != "")
            {
                int resultNumber = int.Parse(RankTextBox.Text + InputNumber(e.Key).ToString());
                if (resultNumber <= 0 || resultNumber > 90)
                {
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.D0 || e.Key == Key.NumPad0)
            {
                e.Handled = true;
            }
        }

        private int InputNumber(Key key)
        {
            int[] inputValue = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int value in inputValue)
            {
                if (key.ToString().EndsWith(value.ToString()))
                {
                    return value;
                }
            }
            throw new Exception("输入越界!");
        }

        private void EliteTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EliteTextBox.Text == "")
            {
                EliteTextBox.Text = "2";
            }
        }

        private void RankTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RankTextBox.Text == "")
            {
                RankTextBox.Text = "1";
            }
        }
    }
}
