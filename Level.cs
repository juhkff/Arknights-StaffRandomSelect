using System;
using System.Collections.Generic;
using System.Text;

namespace StaffRandomSelect
{
    public class Level : Domain.AutomaticNotify
    {
        private int eliteLevel;
        private int rank;
        private string description;

        public int EliteLevel { get => eliteLevel; set => eliteLevel = value; }
        public int Rank { get => rank; set => rank = value; }
        public string Description { get => description; set => ReloadLevel(value); }

        private void ReloadLevel(string newLevelDescription)
        {
            string newEliteStr = newLevelDescription[1].ToString();
            string newRankStr = newLevelDescription.Split(newEliteStr)[1].Split("级")[0];
            int newRank = int.Parse(newRankStr), newEliteLevel;
            switch (newEliteStr)
            {
                case "零":
                    newEliteLevel = 0;
                    break;
                case "一":
                    newEliteLevel = 1;
                    break;
                case "二":
                    newEliteLevel = 2;
                    break;
                default:
                    throw new Exception("范围溢出!");
            }
            SetProperty(ref eliteLevel, newEliteLevel);
            SetProperty(ref rank, newRank);
            SetProperty(ref description, ReloadDescription());
        }

        public Level(int eliteLevel,int rank)
        {
            EliteLevel = eliteLevel;
            Rank = rank;
            GenerateDescription();
        }

        public string ReloadDescription()
        {
            return "精" + EliteLevel switch
            {
                0 => "零",
                1 => "一",
                2 => "二",
                _ => throw new Exception("精英等级越界"),
            } + Rank + "级";
        }

        public void GenerateDescription()
        {
            //return base.ToString();
            Description = "精" + EliteLevel switch
            {
                0 => "零",
                1 => "一",
                2 => "二",
                _ => throw new Exception("精英等级越界"),
            } + Rank + "级";
        }
        
        //默认等级（精二1级）
        public static Level GenerateDefaultLevel()
        {
            return new Level(2, 1);
        }
    }
}