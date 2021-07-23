using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

namespace StaffRandomSelect
{
    /// <summary>
    /// Default.xaml 的交互逻辑
    /// </summary>
    public partial class Generate : Page
    {
        public ObservableCollection<Staff> ResultList { get; }

        public Generate()
        {
            ResultList = new ObservableCollection<Staff>();
            DataContext = this;
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RandomNumText.Text = Slider.Value.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int resultNum = int.Parse(RandomNumText.Text);
            List<Staff> SelectedList = App.staffLists.Where(x => x.IsSelected).Select(x => x).ToList();
            //int length = App.staffLists.Count;
            int length = SelectedList.Count;
            //HashSet<string> nameSet = App.GetNameSet(); //获得名字集合
            HashSet<string> nameSet = SelectedList.Select(staff => staff.Name).ToHashSet(); //获得名字集合
            HashSet<int> indexSet = new HashSet<int>(); //存储生成的索引的集合
            if (nameSet.Count != length)
            {
                //有重复干员名称时：先不做处理
                length = nameSet.Count;
            }
            if (length <= 0 || resultNum > length)
            {
                //无法生成满足要求的结果时
                return;
            }
            //实现功能
            ResultList.Clear();
            Random random = new Random();
            for (int i = 0; i < resultNum; i++)
            {
                int curIndex;
                while (indexSet.Contains(curIndex = random.Next(length))) { };
                indexSet.Add(curIndex);
            }
            //获得index集合
            foreach (int index in indexSet)
            {
                //ResultList.Add(App.staffLists[index]);
                ResultList.Add(SelectedList[index]);
            }
        }
    }
}
