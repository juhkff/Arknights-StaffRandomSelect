using StaffRandomSelect.Domain;
using System.Windows;
using System.Windows.Controls;

namespace StaffRandomSelect
{
    public partial class RandomStrategy : ContentControl
    {
        public RandomStrategy()
        {
            InitializeComponent();
            StrategyItems.ItemsSource = App.RandomStrategyDefinitions;
        }

        private void AddStrategy_Click(object sender, RoutedEventArgs e)
        {
            var draft = new RandomStrategyDefinition();
            var win = new StrategyEditorWindow(draft)
            {
                Owner = Window.GetWindow(this)
            };
            if (win.ShowDialog() == true)
                App.RandomStrategyDefinitions.Add(draft);
        }

        private void EditStrategy_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement)?.Tag is RandomStrategyDefinition def))
                return;

            var win = new StrategyEditorWindow(def)
            {
                Owner = Window.GetWindow(this)
            };
            win.ShowDialog();
            StrategyItems.ItemsSource = null;
            StrategyItems.ItemsSource = App.RandomStrategyDefinitions;
        }

        private void DeleteStrategy_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement)?.Tag is RandomStrategyDefinition def))
                return;

            var r = MessageBox.Show(
                Window.GetWindow(this),
                $"确定删除策略「{def.Name}」？",
                "确认",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (r != MessageBoxResult.Yes)
                return;

            App.RandomStrategyDefinitions.Remove(def);
        }
    }
}
