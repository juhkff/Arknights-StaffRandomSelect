using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StaffRandomSelect
{
    public partial class StaffPickDialog : Window
    {
        private readonly List<Staff> _all;
        private readonly HashSet<string> _excludeNames;

        public IReadOnlyList<string> SelectedStaffNames { get; private set; }

        public StaffPickDialog(Window owner, IEnumerable<string> excludeNames)
        {
            Owner = owner;
            _excludeNames = excludeNames != null
                ? excludeNames.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet()
                : new HashSet<string>();

            _all = App.staffLists
                .GroupBy(s => s.Name)
                .Select(g => g.First())
                .OrderBy(s => s.Name)
                .ToList();

            InitializeComponent();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var q = (SearchBox.Text ?? "").Trim();
            IEnumerable<Staff> src = _all.Where(s => !_excludeNames.Contains(s.Name));
            if (q.Length > 0)
            {
                src = src.Where(s =>
                    s.Name != null &&
                    s.Name.IndexOf(q, System.StringComparison.OrdinalIgnoreCase) >= 0);
            }

            StaffList.ItemsSource = src.ToList();
            StaffList.UnselectAll();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void StaffList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                TryConfirm();
            }
        }

        private void StaffList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => TryConfirm();

        private void Ok_Click(object sender, RoutedEventArgs e) => TryConfirm();

        private void TryConfirm()
        {
            var names = StaffList.SelectedItems
                .Cast<Staff>()
                .Select(s => s.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct()
                .ToList();

            if (names.Count == 0)
            {
                MessageBox.Show(this, "请先在列表中选择至少一名干员（可点击多选）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SelectedStaffNames = names;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
