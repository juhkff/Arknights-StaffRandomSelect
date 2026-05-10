using StaffRandomSelect.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StaffRandomSelect
{
    public class StrategyComboItem
    {
        public string DisplayName { get; set; }
        public RandomStrategyDefinition Model { get; set; }
    }

    /// <summary>
    /// Default.xaml 的交互逻辑
    /// </summary>
    public partial class Generate : ContentControl
    {
        public ObservableCollection<Staff> ResultList { get; }

        public Generate()
        {
            ResultList = new ObservableCollection<Staff>();
            DataContext = this;
            InitializeComponent();
            RandomNumText.Text = ((int)Slider.Value).ToString();
            App.RandomStrategyDefinitions.CollectionChanged += OnStrategiesChanged;
            RefreshStrategyCombo();
        }

        private void OnStrategiesChanged(object sender, NotifyCollectionChangedEventArgs e) => RefreshStrategyCombo();

        private void RefreshStrategyCombo()
        {
            string keepId = (StrategyCombo.SelectedItem as StrategyComboItem)?.Model?.Id;
            var list = new List<StrategyComboItem>
            {
                new StrategyComboItem { DisplayName = "无策略", Model = null }
            };
            foreach (var s in App.RandomStrategyDefinitions)
                list.Add(new StrategyComboItem { DisplayName = s.Name, Model = s });

            StrategyCombo.ItemsSource = list;
            StrategyCombo.SelectedItem = list.FirstOrDefault(x => x.Model?.Id == keepId) ?? list[0];
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RandomNumText.Text = Slider.Value.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int resultNum = int.Parse(RandomNumText.Text);
            var pool = App.staffLists
                .Where(x => x.IsSelected)
                .GroupBy(x => x.Name)
                .Select(g => g.First())
                .ToList();

            if (pool.Count <= 0)
            {
                MessageBox.Show("请先在干员列表中勾选参与随机的干员。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (resultNum > pool.Count)
            {
                MessageBox.Show("随机数量不能大于已选干员人数。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var option = StrategyCombo.SelectedItem as StrategyComboItem;
            var strategy = option?.Model;

            ResultList.Clear();
            var random = new Random();

            if (strategy == null || strategy.Rules == null || strategy.Rules.Count == 0)
            {
                PickUniformNoReplace(pool, resultNum, random);
                return;
            }

            ConstrainedTeamPicker.MergeRules(strategy, out var rarityReq, out var careerReq);
            if (rarityReq.Count == 0 && careerReq.Count == 0)
            {
                PickUniformNoReplace(pool, resultNum, random);
                return;
            }

            if (rarityReq.Values.Sum() > resultNum || careerReq.Values.Sum() > resultNum)
            {
                MessageBox.Show(
                    "策略中要求的稀有度人数或职业人数总和超过了当前「随机数量」，请调整策略或数量。",
                    "无法满足策略",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!ConstrainedTeamPicker.TryPick(pool, resultNum, rarityReq, careerReq, random, out var team))
            {
                MessageBox.Show(
                    "在当前已选干员池下无法凑出满足该策略的阵容，请增加/调整勾选干员或修改策略条目。",
                    "无法满足策略",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            foreach (var s in team)
                ResultList.Add(s);
        }

        private void PickUniformNoReplace(List<Staff> pool, int resultNum, Random random)
        {
            var indexSet = new HashSet<int>();
            int length = pool.Count;
            for (int i = 0; i < resultNum; i++)
            {
                int curIndex;
                while (indexSet.Contains(curIndex = random.Next(length))) { }

                indexSet.Add(curIndex);
            }

            foreach (int index in indexSet)
                ResultList.Add(pool[index]);
        }
    }
}
