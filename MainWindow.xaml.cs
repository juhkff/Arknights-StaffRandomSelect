using StaffRandomSelect;
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

namespace StuffRandomSelect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Generate generate;
        private Input input;
        private StaffRandomSelect.List list;
        public MainWindow()
        {
            generate = new Generate();
            input = new Input();
            list = new StaffRandomSelect.List();
            InitializeComponent();
            ContentControl.Content = generate;
        }

        private void Change_To_Input(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(new Uri("/Input.xaml",UriKind.Relative));
            ContentControl.Content = input;
            Title.Text = "干员录入";
        }

        private void Change_To_Generate(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(new Uri("/Generate.xaml", UriKind.Relative));
            ContentControl.Content = generate;
            Title.Text = "阵容生成";
        }

        private void Change_To_List(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(new Uri("/List.xaml", UriKind.Relative));
            ContentControl.Content = list;
            Title.Text = "干员列表";
        }
    }
}
