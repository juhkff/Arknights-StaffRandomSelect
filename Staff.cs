namespace StaffRandomSelect
{
    public class Staff : Domain.AutomaticNotify
    {
        private string name;
        private int star;
        private Career career;
        private bool isSelected;


        public Staff() { }

        public Staff(string name, int star, Career career)
        {
            Name = name;
            Star = star;
            Career = career;
            IsSelected = false;
        }

        public string Name { get => name; set => SetProperty(ref name, value); }
        public int Star { get => star; set => SetProperty(ref star, value); }
        public Career Career { get => career; set => SetProperty(ref career, value); }
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
    }
}
