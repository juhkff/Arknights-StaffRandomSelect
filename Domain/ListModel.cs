using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace StaffRandomSelect.Domain
{
    public class ListModel : AutomaticNotify
    {
        public ObservableCollection<Staff> StaffList { get; }
        public ListModel()
        {
            StaffList = App.staffLists;
            foreach (var staff in StaffList)
            {
                staff.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(Staff.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllStaffSelected));
                    }
                };
            }
        }

        public bool? IsAllStaffSelected
        {
            get
            {
                List<bool> selected = StaffList.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, StaffList);
                    OnPropertyChanged();
                }
            }
        }

        private static void SelectAll(bool select,IEnumerable<Staff> staffs)
        {
            foreach (Staff staff in staffs)
            {
                staff.IsSelected = select;
            }
        }
    }
}
