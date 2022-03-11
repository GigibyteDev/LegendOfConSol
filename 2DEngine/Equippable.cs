using System;
using System.Collections.Generic;
using System.Text;

namespace _2DEngine
{
    public class Equippable : IItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Strength { get; set; }
        public bool Equipped { get; set; }

        public Equippable(string name, string type, int strength)
        {
            Name = name;
            Type = type;
            Strength = strength;
            Equipped = false;
        }
    }
}