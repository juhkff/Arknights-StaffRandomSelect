using StaffRandomSelect.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StaffRandomSelect
{
    public partial class StrategyEditorWindow : Window
    {
        private readonly RandomStrategyDefinition _target;
        private readonly string _backupName;
        private readonly List<StrategyRule> _backupRules;

        public StrategyEditorWindow(RandomStrategyDefinition target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _backupName = _target.Name;
            _backupRules = _target.Rules.Select(CloneRule).ToList();

            InitializeComponent();
            NameBox.Text = _target.Name;
            RulesList.ItemsSource = _target.Rules;

            StarCombo.ItemsSource = Enumerable.Range(1, 6).ToList();
            StarCombo.SelectedItem = 6;

            CareerCombo.ItemsSource = Enum.GetValues(typeof(Career));
        }

        private static StrategyRule CloneRule(StrategyRule r) =>
            new StrategyRule
            {
                Kind = r.Kind,
                Star = r.Star,
                Career = r.Career,
                Count = r.Count
            };

        private void RestoreBackup()
        {
            _target.Name = _backupName;
            _target.Rules.Clear();
            foreach (var r in _backupRules)
                _target.Rules.Add(CloneRule(r));
        }

        private void RemoveRule_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is StrategyRule rule)
                _target.Rules.Remove(rule);
        }

        private void AddRarityRule_Click(object sender, RoutedEventArgs e)
        {
            if (!(StarCombo.SelectedItem is int star))
                return;
            if (!int.TryParse(RarityCountBox.Text?.Trim(), out int n) || n <= 0)
            {
                MessageBox.Show(this, "请输入正确的稀有度数量（正整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _target.Rules.Add(new StrategyRule { Kind = StrategyRuleKind.Rarity, Star = star, Count = n });
        }

        private void AddCareerRule_Click(object sender, RoutedEventArgs e)
        {
            if (!(CareerCombo.SelectedItem is Career career))
                return;
            if (!int.TryParse(CareerCountBox.Text?.Trim(), out int n) || n <= 0)
            {
                MessageBox.Show(this, "请输入正确的职业数量（正整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _target.Rules.Add(new StrategyRule { Kind = StrategyRuleKind.Career, Career = career, Count = n });
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(this, "请填写策略名称。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _target.Name = name;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RestoreBackup();
            DialogResult = false;
            Close();
        }
    }
}
