using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StaffRandomSelect.Domain
{
    public class ListModel
    {
        public ObservableCollection<Test> Items1 { get; }
        public ListModel()
        {
            Items1 = new ObservableCollection<Test>();
            Test test1 = new Test();
            test1.Code = "Code1";
            test1.Name = "Name1";
            test1.Description = "Description1";
            Items1.Add(test1);
        }
    }

    public class Test : INotifyPropertyChanged
    {
        private string code;
        private string name;
        private string description;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Test() { }
        public Test(string code,string name,string description)
        {
            Code = code;
            Name = name;
            Description = description;
        }

        public string Code { get => code; set => SetProperty(ref code, value); }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }



        public bool SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return false;
            }
            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
