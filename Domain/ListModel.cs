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
        }

        public bool? IsAllStaffSelected
        {
            get
            {
                var selected = StaffList.Select(item => item.IsSelected).Distinct().ToList();
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
            foreach(var staff in staffs)
            {
                staff.IsSelected = select;
            }
        }
    }

    //public class Staff : INotifyPropertyChanged
    //{
    //    private string code;
    //    private string name;
    //    private string description;

    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    public Staff() { }
    //    public Staff(string code,string name,string description)
    //    {
    //        Code = code;
    //        Name = name;
    //        Description = description;
    //    }

    //    public string Code { get => code; set => SetProperty(ref code, value); }
    //    public string Name { get => name; set => SetProperty(ref name, value); }
    //    public string Description { get => description; set => SetProperty(ref description, value); }



    //    public bool SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
    //    {
    //        if (EqualityComparer<T>.Default.Equals(property, value))
    //        {
    //            return false;
    //        }
    //        property = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }

    //    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    //}
}
