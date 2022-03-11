namespace _2DEngine
{
    public class WorldObject : IObstacle, IInteract
    {
        public string Name { get; set; }
        public int LocLR { get; set; }
        public int LocUD { get; set; }
        public string[] Icon { get; set; }
        public bool Interactible { get; set; }

        public WorldObject(string name, int locLR, int locUD, string[] icon, bool interactible = false)
        {
            Name = name;
            LocLR = locLR;
            LocUD = locUD;
            Icon = icon;
            Interactible = interactible;
        }

        public WorldObject(string name, string[] icon)
        {
            Name = name;
            Icon = icon;
            LocLR = Engine.width / 2;
            LocUD = Engine.height / 2;
            Interactible = false;
        }
    }
}
