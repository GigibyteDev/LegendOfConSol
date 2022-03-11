namespace _2DEngine
{
    public interface IObstacle
    {
        string Name { get; set; }
        int LocLR { get; set; }
        int LocUD { get; set; }
        string[] Icon { get; set; }
        bool Interactible { get; set; }
    }
}
