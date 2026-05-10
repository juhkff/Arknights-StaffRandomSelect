using StaffRandomSelect.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StaffRandomSelect
{
    public partial class StrategyEditorWindow : Window
    {
        private readonly RandomStrategyDefinition _target;
        private readonly string _backupName;
        private readonly List<StrategyRule> _backupRules;
        private readonly List<string> _staffSubsetDraft = new List<string>();
        private StrategyRule _editingRule;
        private bool _rulesListSelectionSuppress;

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
            CareerCombo.SelectedItem = Career.先锋;

            // 必须在 InitializeComponent 之后订阅：若在 XAML 里写 SelectionChanged，设置 SelectedIndex 时
            // 下方 CareerExactPanel 等尚未构造，会空引用崩溃。
            CareerConstraintModeCombo.SelectionChanged += CareerConstraintModeCombo_SelectionChanged;

            StaffSubsetModeCombo.SelectionChanged += StaffSubsetModeCombo_SelectionChanged;

            WireDigitOnlyTextBoxes();
            UpdateStaffSubsetInputPanels();
            RefreshStaffSubsetTagPanel();
            UpdateSubmitButtonLabels();
        }

        private void RulesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_rulesListSelectionSuppress)
                return;
            if (RulesList.SelectedItem is StrategyRule r)
            {
                _editingRule = r;
                LoadRuleIntoForms(r);
                UpdateSubmitButtonLabels();
                CancelRuleEditButton.Visibility = Visibility.Visible;
            }
            else
            {
                _editingRule = null;
                ClearStrategyEntryForms();
                UpdateSubmitButtonLabels();
                CancelRuleEditButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelRuleEdit_Click(object sender, RoutedEventArgs e) => ClearSelectionAndEdit();

        private void ClearSelectionAndEdit()
        {
            _editingRule = null;
            _rulesListSelectionSuppress = true;
            RulesList.SelectedItem = null;
            _rulesListSelectionSuppress = false;
            ClearStrategyEntryForms();
            UpdateSubmitButtonLabels();
            CancelRuleEditButton.Visibility = Visibility.Collapsed;
        }

        private void DetachComboHandlers()
        {
            CareerConstraintModeCombo.SelectionChanged -= CareerConstraintModeCombo_SelectionChanged;
            StaffSubsetModeCombo.SelectionChanged -= StaffSubsetModeCombo_SelectionChanged;
        }

        private void AttachComboHandlers()
        {
            CareerConstraintModeCombo.SelectionChanged += CareerConstraintModeCombo_SelectionChanged;
            StaffSubsetModeCombo.SelectionChanged += StaffSubsetModeCombo_SelectionChanged;
        }

        private void ClearStrategyEntryFieldsInner()
        {
            StarCombo.SelectedItem = 6;
            RarityCountBox.Text = "";
            CareerCombo.SelectedItem = Career.先锋;
            CareerConstraintModeCombo.SelectedIndex = 0;
            CareerCountBox.Text = "";
            CareerRangeMinBox.Text = "";
            CareerRangeMaxBox.Text = "";
            _staffSubsetDraft.Clear();
            StaffSubsetModeCombo.SelectedIndex = 0;
            StaffSubsetExactCountBox.Text = "";
            StaffSubsetRangeMinBox.Text = "";
            StaffSubsetRangeMaxBox.Text = "";
        }

        private void ClearStrategyEntryForms()
        {
            DetachComboHandlers();
            try
            {
                ClearStrategyEntryFieldsInner();
            }
            finally
            {
                AttachComboHandlers();
                UpdateCareerConstraintPanels();
                UpdateStaffSubsetInputPanels();
            }

            RefreshStaffSubsetTagPanel();
        }

        private void LoadRuleIntoForms(StrategyRule r)
        {
            DetachComboHandlers();
            try
            {
                ClearStrategyEntryFieldsInner();
                switch (r.Kind)
                {
                    case StrategyRuleKind.Rarity:
                        StarCombo.SelectedItem = r.Star;
                        RarityCountBox.Text = r.Count.ToString();
                        break;
                    case StrategyRuleKind.Career:
                        CareerCombo.SelectedItem = r.Career;
                        CareerConstraintModeCombo.SelectedIndex = 0;
                        CareerCountBox.Text = r.Count.ToString();
                        break;
                    case StrategyRuleKind.CareerRange:
                        CareerCombo.SelectedItem = r.Career;
                        CareerConstraintModeCombo.SelectedIndex = 1;
                        CareerRangeMinBox.Text = r.Count.ToString();
                        CareerRangeMaxBox.Text = r.CountMax.ToString();
                        break;
                    case StrategyRuleKind.StaffSubsetExact:
                        if (r.StaffNames != null)
                        {
                            foreach (var n in r.StaffNames)
                            {
                                if (!string.IsNullOrWhiteSpace(n) && !_staffSubsetDraft.Contains(n.Trim()))
                                    _staffSubsetDraft.Add(n.Trim());
                            }
                        }

                        StaffSubsetModeCombo.SelectedIndex = 0;
                        StaffSubsetExactCountBox.Text = r.Count.ToString();
                        break;
                    case StrategyRuleKind.StaffSubsetRange:
                        if (r.StaffNames != null)
                        {
                            foreach (var n in r.StaffNames)
                            {
                                if (!string.IsNullOrWhiteSpace(n) && !_staffSubsetDraft.Contains(n.Trim()))
                                    _staffSubsetDraft.Add(n.Trim());
                            }
                        }

                        StaffSubsetModeCombo.SelectedIndex = 1;
                        StaffSubsetRangeMinBox.Text = r.Count.ToString();
                        StaffSubsetRangeMaxBox.Text = r.CountMax.ToString();
                        break;
                }
            }
            finally
            {
                AttachComboHandlers();
                UpdateCareerConstraintPanels();
                UpdateStaffSubsetInputPanels();
                RefreshStaffSubsetTagPanel();
            }
        }

        private void UpdateSubmitButtonLabels()
        {
            if (RarityRuleSubmitButton == null)
                return;
            RarityRuleSubmitButton.Content = _editingRule?.Kind == StrategyRuleKind.Rarity ? "保存" : "添加";
            bool careerEdit = _editingRule?.Kind == StrategyRuleKind.Career || _editingRule?.Kind == StrategyRuleKind.CareerRange;
            CareerRuleSubmitButton.Content = careerEdit ? "保存" : "添加";
            bool staffEdit = _editingRule?.Kind == StrategyRuleKind.StaffSubsetExact || _editingRule?.Kind == StrategyRuleKind.StaffSubsetRange;
            StaffSubsetRuleSubmitButton.Content = staffEdit ? "保存" : "添加";
        }

        private void ReplaceRuleAt(StrategyRule oldRule, StrategyRule newRule)
        {
            int idx = _target.Rules.IndexOf(oldRule);
            if (idx < 0)
                return;
            _target.Rules.RemoveAt(idx);
            _target.Rules.Insert(idx, newRule);
            ClearSelectionAndEdit();
        }

        private void CareerConstraintModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            UpdateCareerConstraintPanels();

        private void UpdateCareerConstraintPanels()
        {
            if (CareerCountBox == null || CareerRangeMinBox == null || CareerRangeMaxBox == null || CareerConstraintModeCombo == null)
                return;
            bool exact = CareerConstraintModeCombo.SelectedIndex == 0;
            CareerCountBox.Visibility = exact ? Visibility.Visible : Visibility.Collapsed;
            CareerRangeMinBox.Visibility = exact ? Visibility.Collapsed : Visibility.Visible;
            CareerRangeMaxBox.Visibility = exact ? Visibility.Collapsed : Visibility.Visible;
        }

        private void WireDigitOnlyTextBoxes()
        {
            foreach (var tb in new[]
                     {
                         RarityCountBox, CareerCountBox, CareerRangeMinBox, CareerRangeMaxBox,
                         StaffSubsetExactCountBox, StaffSubsetRangeMinBox, StaffSubsetRangeMaxBox
                     })
            {
                tb.PreviewTextInput += DigitOnlyTextBox_PreviewTextInput;
                tb.PreviewKeyDown += DigitOnlyTextBox_PreviewKeyDown;
                tb.TextChanged += DigitOnlyTextBox_TextChanged;
                DataObject.AddPastingHandler(tb, DigitOnlyTextBox_Pasting);
            }
        }

        private void StaffSubsetModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            UpdateStaffSubsetInputPanels();

        private void UpdateStaffSubsetInputPanels()
        {
            if (StaffSubsetExactCountBox == null || StaffSubsetRangeInputPanel == null || StaffSubsetModeCombo == null)
                return;
            bool exact = StaffSubsetModeCombo.SelectedIndex == 0;
            StaffSubsetExactCountBox.Visibility = exact ? Visibility.Visible : Visibility.Collapsed;
            StaffSubsetRangeInputPanel.Visibility = exact ? Visibility.Collapsed : Visibility.Visible;
        }

        private void RefreshStaffSubsetTagPanel()
        {
            if (StaffSubsetTagsPanel == null)
                return;
            StaffSubsetTagsPanel.Children.Clear();
            var tagBg = TryFindResource("PrimaryHueLightBrush") as Brush
                        ?? new SolidColorBrush(Color.FromRgb(237, 231, 246));

            foreach (var name in _staffSubsetDraft)
            {
                var border = new Border
                {
                    Background = tagBg,
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(8, 3, 4, 3),
                    Margin = new Thickness(0, 0, 6, 4),
                    VerticalAlignment = VerticalAlignment.Center
                };
                var row = new StackPanel { Orientation = Orientation.Horizontal };
                row.Children.Add(new TextBlock
                {
                    Text = name,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = new FontFamily("Microsoft YaHei"),
                    Margin = new Thickness(0, 0, 4, 0)
                });
                var remove = new Button
                {
                    Content = "×",
                    Padding = new Thickness(4, 0, 4, 0),
                    Tag = name,
                    FontFamily = new FontFamily("Microsoft YaHei"),
                    Cursor = Cursors.Hand,
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (TryFindResource("MaterialDesignFlatButton") is Style st)
                    remove.Style = st;
                remove.Click += RemoveStaffSubsetTag_Click;
                row.Children.Add(remove);
                border.Child = row;
                StaffSubsetTagsPanel.Children.Add(border);
            }
        }

        private void RemoveStaffSubsetTag_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement)?.Tag is string name))
                return;
            _staffSubsetDraft.Remove(name);
            RefreshStaffSubsetTagPanel();
        }

        private void StaffSubsetAddButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new StaffPickDialog(this, _staffSubsetDraft);
            if (dlg.ShowDialog() != true || dlg.SelectedStaffNames == null || dlg.SelectedStaffNames.Count == 0)
                return;

            var nameSet = App.GetNameSet();
            foreach (var n in dlg.SelectedStaffNames)
            {
                var trimmed = n.Trim();
                if (!nameSet.Contains(trimmed))
                {
                    MessageBox.Show(this, $"干员「{trimmed}」未在干员列表中录入，已跳过。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    continue;
                }

                if (_staffSubsetDraft.Contains(trimmed))
                    continue;
                _staffSubsetDraft.Add(trimmed);
            }

            RefreshStaffSubsetTagPanel();
        }

        private void AddStaffSubsetRule_Click(object sender, RoutedEventArgs e)
        {
            if (_staffSubsetDraft.Count == 0)
            {
                MessageBox.Show(this, "请至少选择一名干员。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_editingRule != null)
            {
                if (_editingRule.Kind != StrategyRuleKind.StaffSubsetExact && _editingRule.Kind != StrategyRuleKind.StaffSubsetRange)
                {
                    MessageBox.Show(this, "当前选中条目不是干员池限制类型。请先点「取消编辑」或选择对应条目后再操作。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            var nameSet = App.GetNameSet();
            foreach (var n in _staffSubsetDraft)
            {
                if (!nameSet.Contains(n))
                {
                    MessageBox.Show(this, $"干员「{n}」未在干员列表中录入。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            var names = new List<string>(_staffSubsetDraft);

            if (StaffSubsetModeCombo.SelectedIndex == 0)
            {
                if (!int.TryParse(StaffSubsetExactCountBox.Text?.Trim(), out int cn) || cn < 0)
                {
                    MessageBox.Show(this, "请输入正确的固定人数（非负整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (cn > names.Count)
                {
                    MessageBox.Show(this, "固定人数不能大于已选干员种类数。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var newRule = new StrategyRule
                {
                    Kind = StrategyRuleKind.StaffSubsetExact,
                    StaffNames = names,
                    Count = cn
                };
                if (_editingRule != null)
                {
                    ReplaceRuleAt(_editingRule, newRule);
                    return;
                }

                _target.Rules.Add(newRule);
            }
            else
            {
                if (!int.TryParse(StaffSubsetRangeMinBox.Text?.Trim(), out int lo) || lo < 0)
                {
                    MessageBox.Show(this, "请输入正确的范围下限（非负整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!int.TryParse(StaffSubsetRangeMaxBox.Text?.Trim(), out int hi))
                {
                    MessageBox.Show(this, "请输入正确的范围上限（整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (lo > hi)
                {
                    MessageBox.Show(this, "范围下限不能大于上限。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (hi > names.Count)
                {
                    MessageBox.Show(this, "范围上限不能大于已选干员种类数。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var newRuleR = new StrategyRule
                {
                    Kind = StrategyRuleKind.StaffSubsetRange,
                    StaffNames = names,
                    Count = lo,
                    CountMax = hi
                };
                if (_editingRule != null)
                {
                    ReplaceRuleAt(_editingRule, newRuleR);
                    return;
                }

                _target.Rules.Add(newRuleR);
            }

            _staffSubsetDraft.Clear();
            RefreshStaffSubsetTagPanel();
        }

        private static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';

        private static void DigitOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(ch => !IsAsciiDigit(ch));
        }

        private static void DigitOnlyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static void DigitOnlyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox tb))
                return;
            var raw = tb.Text ?? "";
            if (raw.Length == 0 || raw.All(IsAsciiDigit))
                return;
            int selStart = tb.SelectionStart;
            int keptBeforeCaret = raw.Take(selStart).Count(IsAsciiDigit);
            var filtered = new string(raw.Where(IsAsciiDigit).ToArray());
            tb.Text = filtered;
            tb.SelectionStart = Math.Min(Math.Max(0, keptBeforeCaret), filtered.Length);
        }

        private static void DigitOnlyTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
                return;
            var text = e.DataObject.GetData(DataFormats.Text) as string ?? "";
            if (text.Any(ch => !IsAsciiDigit(ch)))
                e.CancelCommand();
        }

        private static StrategyRule CloneRule(StrategyRule r) =>
            new StrategyRule
            {
                Kind = r.Kind,
                Star = r.Star,
                Career = r.Career,
                Count = r.Count,
                CountMax = r.CountMax,
                StaffNames = r.StaffNames != null ? new List<string>(r.StaffNames) : new List<string>()
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
            if (!((sender as FrameworkElement)?.Tag is StrategyRule rule))
                return;
            if (rule == _editingRule)
            {
                _rulesListSelectionSuppress = true;
                _target.Rules.Remove(rule);
                _editingRule = null;
                RulesList.SelectedItem = null;
                _rulesListSelectionSuppress = false;
                ClearStrategyEntryForms();
                UpdateSubmitButtonLabels();
                CancelRuleEditButton.Visibility = Visibility.Collapsed;
                return;
            }

            _target.Rules.Remove(rule);
        }

        private void AddRarityRule_Click(object sender, RoutedEventArgs e)
        {
            if (_editingRule != null && _editingRule.Kind != StrategyRuleKind.Rarity)
            {
                MessageBox.Show(this, "当前选中条目不是「固定特定稀有度总量」类型。请先点「取消编辑」或选择对应条目后再操作。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!(StarCombo.SelectedItem is int star))
                return;
            if (!int.TryParse(RarityCountBox.Text?.Trim(), out int n) || n <= 0)
            {
                MessageBox.Show(this, "请输入正确的稀有度数量（正整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newRule = new StrategyRule { Kind = StrategyRuleKind.Rarity, Star = star, Count = n };
            if (_editingRule != null)
                ReplaceRuleAt(_editingRule, newRule);
            else
                _target.Rules.Add(newRule);
        }

        private void AddCareerConstraint_Click(object sender, RoutedEventArgs e)
        {
            if (!(CareerCombo.SelectedItem is Career career))
                return;

            if (_editingRule != null)
            {
                if (_editingRule.Kind != StrategyRuleKind.Career && _editingRule.Kind != StrategyRuleKind.CareerRange)
                {
                    MessageBox.Show(this, "当前选中条目不是职业限制类型。请先点「取消编辑」或选择对应条目后再操作。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            if (CareerConstraintModeCombo.SelectedIndex == 0)
            {
                if (!int.TryParse(CareerCountBox.Text?.Trim(), out int n) || n <= 0)
                {
                    MessageBox.Show(this, "请输入正确的职业数量（正整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var newRule = new StrategyRule { Kind = StrategyRuleKind.Career, Career = career, Count = n };
                if (_editingRule != null)
                    ReplaceRuleAt(_editingRule, newRule);
                else
                    _target.Rules.Add(newRule);
                return;
            }

            if (!int.TryParse(CareerRangeMinBox.Text?.Trim(), out int lo) || lo < 0)
            {
                MessageBox.Show(this, "请输入正确的范围下限（非负整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(CareerRangeMaxBox.Text?.Trim(), out int hi))
            {
                MessageBox.Show(this, "请输入正确的范围上限（整数）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (lo > hi)
            {
                MessageBox.Show(this, "范围下限不能大于上限。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newRangeRule = new StrategyRule
            {
                Kind = StrategyRuleKind.CareerRange,
                Career = career,
                Count = lo,
                CountMax = hi
            };
            if (_editingRule != null)
                ReplaceRuleAt(_editingRule, newRangeRule);
            else
                _target.Rules.Add(newRangeRule);
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
