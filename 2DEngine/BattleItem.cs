using System;
using System.Collections.Generic;
using System.Text;

namespace _2DEngine
{
    public class BattleItem : IItem
    {
        public string Name { get; set; }
        public int Amt { get; set; }
        public int MaxAmt { get; set; } = 99;
        public string Type { get; set; }

        public int Strength { get; set; }

        public BattleItem(string name, string type, int strength, int amt)
        {
            Name = name;
            Type = type;
            Strength = strength;
            Amt = amt;
        }

    }
}