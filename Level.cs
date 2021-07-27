using System;
using System.Collections.Generic;
using System.Text;

namespace StaffRandomSelect
{
    public class Level
    {
        private int eliteLevel;
        private int rank;

        public int EliteLevel { get => eliteLevel; set => eliteLevel = value; }
        public int Rank { get => rank; set => rank = value; }

        public override string ToString()
        {
            string elite = EliteLevel switch
            {
                0 => "零",
                1 => "一",
                2 => "二",
                _ => throw new Exception("精英等级越界"),
            };
            //return base.ToString();
            return "精" + elite + Rank + "级";
        }
        
        //默认等级（精二1级）
        public static Level generateDefaultLevel()
        {
            return new Level { eliteLevel = 2, rank = 1 };
        }
    }
}
