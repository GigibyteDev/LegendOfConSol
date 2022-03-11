using System.Collections.Generic;

namespace _2DEngine
{
    public class NPC : IObstacle, IInteract
    {
        public string Name { get; set; }
        public int LocLR { get; set; }
        public int LocUD { get; set; }
        public string[] Icon { get; set; }
        public bool Interactible { get; set; }
        public Dictionary<string, List<string>> Dialogue { get; set; } = new Dictionary<string, List<string>>();
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Randomness { get; set; }

        public NPC(string name, int locLR, int locUD, string[] icon, Dictionary<string, List<string>> dialogue, int health, int strength, bool interactible = true)
        {
            Name = name;
            LocLR = locLR;
            LocUD = locUD;
            Icon = icon;
            Dialogue = dialogue;
            Health = health;
            Strength = strength;
            Randomness = 5;
            Interactible = interactible;
        }

        public NPC(string name, int locLR, int locUD, string[] icon, Dictionary<string, List<string>> dialogue, int health, int strength, int randomness, bool interactible = true)
        {
            Name = name;
            LocLR = locLR;
            LocUD = locUD;
            Icon = icon;
            Dialogue = dialogue;
            Health = health;
            Strength = strength;
            Randomness = randomness;
            Interactible = interactible;
        }

        public NPC(string name, int locLR, int locUD, string[] icon, int health, int strength, bool interactible = true)
        {
            Name = name;
            LocLR = locLR;
            LocUD = locUD;
            Icon = icon;
            Health = health;
            Strength = strength;
            Interactible = interactible;
        }

        public NPC(string name, string[] icon)
        {
            Name = name;
            Icon = icon;
            LocLR = Engine.width / 2;
            LocUD = Engine.height / 2;
            Interactible = false;
        }
    }
}
