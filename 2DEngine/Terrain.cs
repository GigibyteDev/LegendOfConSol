using System;

namespace _2DEngine
{
    public class Terrain : IInteract
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.White;
        public ConsoleColor TextColor { get; set; } = ConsoleColor.Black;

        public Vector2 CornerUL { get; set; } = new Vector2(0, 0);
        public Vector2 CornerLR { get; set; } = new Vector2(Console.WindowWidth, Console.WindowHeight);
        public bool Walkable { get; set; } = true;
        public bool Interactible { get; set; } = false;
        public bool Visible { get; set; } = true;
    }
}
