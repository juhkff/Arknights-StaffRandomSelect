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

            ConstrainedTeamPicker.MergeRules(strategy, out var rarityReq, out var careerExact, out var careerRange, out var staffSubsets);
            if (rarityReq.Count == 0 && careerExact.Count == 0 && careerRange.Count == 0 && staffSubsets.Count == 0)
            {
                PickUniformNoReplace(pool, resultNum, random);
                return;
            }

            if (careerRange.Values.Any(x => x.lo > x.hi))
            {
                MessageBox.Show(
                    "策略中存在互相冲突的职业数量范围条目（交集为空），请修改后重试。",
                    "无法满足策略",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (rarityReq.Values.Sum() > resultNum || careerExact.Values.Sum() > resultNum)
            {
                MessageBox.Show(
                    "策略中要求的稀有度人数或职业人数总和超过了当前「随机数量」，请调整策略或数量。",
                    "无法满足策略",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            int minCareer = 0;
            foreach (Career career in Enum.GetValues(typeof(Career)))
            {
                if (careerExact.TryGetValue(career, out int ex))
                    minCareer += ex;
                else if (careerRange.TryGetValue(career, out var rg))
                    minCareer += rg.lo;
            }

            if (minCareer > resultNum)
            {
                MessageBox.Show(
                    "策略中各职业数量（及范围下限）之和超过了当前「随机数量」，请调整策略或数量。",
                    "无法满足策略",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            foreach (var c in staffSubsets)
            {
                int inPool = pool.Count(s => c.Names.Contains(s.Name));
                int outPool = pool.Count(s => !c.Names.Contains(s.Name));
                if (c.IsExact)
                {
                    int n = c.ExactOrLo;
                    if (n > resultNum)
                    {
                        MessageBox.Show(
                            "「限制特定干员人数」的固定值超过了当前「随机数量」。",
                            "无法满足策略",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    if (n > inPool)
                    {
                        MessageBox.Show(
                            "「限制特定干员人数」：在已选干员池中，指定干员不足以满足固定人数，请调整勾选或策略。",
                            "无法满足策略",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    if (n == 0 && outPool < resultNum)
                    {
                        MessageBox.Show(
                            "「限制特定干员人数」为 0 时，需要足够多的「非指定」已选干员填满阵容，请调整勾选或策略。",
                            "无法满足策略",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    if (c.ExactOrLo > inPool || c.Hi > resultNum || c.ExactOrLo > resultNum)
                    {
                        MessageBox.Show(
                            "「限制特定干员人数」的范围与当前已选干员池或随机数量不兼容，请调整。",
                            "无法满足策略",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (!ConstrainedTeamPicker.TryPick(pool, resultNum, rarityReq, careerExact, careerRange, staffSubsets, random, out var team))
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
