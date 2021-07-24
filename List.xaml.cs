using StaffRandomSelect.Domain;
using System;
using System.Collections.Generic;
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
    /// List.xaml 的交互逻辑
    /// </summary>
    public partial class List : ContentControl
    {
        public List()
        {
            DataContext = new ListModel();
            InitializeComponent();
            ComboBoxColumn.ItemsSource = Enum.GetValues(typeof(Career));
            StarColumn.ItemsSource = new int[] { 1, 2, 3, 4, 5, 6 };
            
        }

        private void LoadList()
        {
            //foreach(Staff staff in App.StaffLists)
            //{
            //    TextBlock nameBlock = new TextBlock();
            //    nameBlock.Text = staff.Name;
            //}
        }

        
    }
}
