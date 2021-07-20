using System;
using System.Collections.Generic;
using System.Text;

namespace StaffRandomSelect
{
    public class Staff
    {
        private string name;
        private int star;
        private Career career;



        public Staff() { }
        
        public Staff(string name,int star,Career career)
        {
            this.Name = name;
            this.Star = star;
            this.Career = career;
        }

        public string Name { get => name; set => name = value; }
        public int Star { get => star; set => star = value; }
        public Career Career { get => career; set => career = value; }
    }
}
