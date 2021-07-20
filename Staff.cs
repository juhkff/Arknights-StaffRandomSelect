using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StaffRandomSelect
{
    public class Staff : INotifyPropertyChanged
    {
        private string name;
        private int star;
        private Career career;
        public event PropertyChangedEventHandler PropertyChanged;


        public Staff() { }

        public Staff(string name, int star, Career career)
        {
            Name = name;
            Star = star;
            Career = career;
        }

        public string Name { get => name; set => SetProperty(ref name, value); }
        public int Star { get => star; set => SetProperty(ref star, value); }
        public Career Career { get => career; set => SetProperty(ref career, value); }

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
