using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StuffRandomSelect
{
    /// <summary>
    /// Default.xaml 的交互逻辑
    /// </summary>
    public partial class Generate : Page
    {
        public Generate()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RandomNumText.Text = Slider.Value.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> lists = new List<string>();
            string projectPath = Environment.CurrentDirectory.ToString();
            string fileName = "StaffList.txt";
            string path;
            if (!Directory.Exists(projectPath))
            {
                throw new Exception("程序异常");
            }
            else
            {
                path = System.IO.Path.Combine(projectPath, fileName);
                //暂不考虑是否存在文件
                if (File.Exists(path))
                {
                    StreamReader sr = new StreamReader(path, Encoding.Default);
                    string line = null, nextLine = null;
                    while ((nextLine = sr.ReadLine()) != null)
                    {
                        if (nextLine.Length <= 0)
                        {
                            continue;
                        }
                        line = nextLine;
                        lists.Add(line.Split("\t")[0]);     //lists的处理也是日后的工作
                    }
                    int randomNum = int.Parse(RandomNumText.Text);
                    if (randomNum > lists.Count)
                    {
                        //数量不够
                        //日后处理
                    }
                    else
                    {
                        //随机生成
                        int curNum = 0;
                        HashSet<int> indexSet = new HashSet<int>();
                        Random random = new Random();
                        while (curNum < randomNum)
                        {
                            int newIndex=random.Next(0, lists.Count);
                            if (indexSet.Contains(newIndex))
                            {
                                continue;
                            }
                            else
                            {
                                indexSet.Add(newIndex);
                                curNum++;
                            }
                        }
                        //结束生成
                        string result = "";
                        foreach(int cur in indexSet)
                        {
                            result += lists[cur] + "\t";
                        }
                        ResultBlock.Text = result;
                    }
                }
            }
        }
    }
}
