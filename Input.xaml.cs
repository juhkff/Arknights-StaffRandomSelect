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
                if (timer == null)
                {
                    SnackbarTwo.IsActive = true;
                }
                else if (timer != null)
                {
                    SnackbarOne.IsActive = false;
                    SnackbarTwo.IsActive = false;
                    SnackbarThree.IsActive = false;
                    SnackbarTwo.IsActive = true;
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
                return;
            }
            //正常时
            ModifyList(NameTextBox.Text, StarBar.Value, (Career)Enum.Parse(typeof(Career),ComboBox.Text));
        }


        private void ModifyList(string name,int star,Career career)
        {

            //string projectPath = Environment.CurrentDirectory.ToString();
            //string fileName = "StaffList.txt";
            //string path;
            //if (!Directory.Exists(projectPath))
            //{
            //    throw new Exception("程序异常");
            //}
            //else
            //{
            //    path = System.IO.Path.Combine(projectPath, fileName);
            //    if (!File.Exists(path))
            //    {
            //        File.Create(path).Close();  //创建文件
            //    }
            //    StreamReader sr = new StreamReader(path, Encoding.Default);
            //    string line = null, nextLine = null;
            //    while ((nextLine = sr.ReadLine()) != null)
            //    {
            //        if (nextLine.Length <= 0)
            //        {
            //            continue;
            //        }
            //        line = nextLine;
            //        //line为每一行的文本
            //        string curName = line.Split("\t")[0];
            //        string newName = name.Split("\t")[0];
            //        if (curName == newName)
            //        {
            //            if (timer == null)
            //            {
            //                SnackbarOne.IsActive = true;
            //            }
            //            else if (timer != null)
            //            {
            //                SnackbarOne.IsActive = false;
            //                SnackbarTwo.IsActive = false;
            //                SnackbarThree.IsActive = false;
            //                SnackbarOne.IsActive = true;
            //                timer.Stop();
            //                timer = null;
            //            }
            //            timer = new Timer();
            //            timer.Interval = 2000;
            //            timer.Elapsed += new System.Timers.ElapsedEventHandler(MyEventHandler);
            //            timer.AutoReset = false;
            //            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            //            return;
            //        }
            //    }
            //    if (line != null && line != "\n")
            //    {
            //        sr.Close();
            //        string[] temp = new string[1];
            //        temp[0] = "\n";
            //        File.AppendAllLines(path, temp);    //确保以换行符结尾
            //    }
            //    sr.Close();
            //    //无重复名，则追加行
            //    string[] result = name.Split("\n");
            //    File.AppendAllLines(path, result);
            //    if (timer == null)
            //    {
            //        SnackbarThree.IsActive = true;
            //    }
            //    else if (timer != null)
            //    {
            //        SnackbarOne.IsActive = false;
            //        SnackbarTwo.IsActive = false;
            //        SnackbarThree.IsActive = false;
            //        SnackbarThree.IsActive = true;
            //        timer.Stop();
            //        timer = null;
            //    }
            //    timer = new Timer();
            //    timer.Interval = 2000;
            //    timer.Elapsed += new System.Timers.ElapsedEventHandler(MyEventHandler);
            //    timer.AutoReset = false;
            //    timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            //    return;
            //}
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
