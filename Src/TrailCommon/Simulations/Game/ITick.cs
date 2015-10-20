namespace TrailCommon
{
    public interface ITick
    {
        Randomizer Random { get; }
        event FirstTick FirstTickEvent;
        event TickSim TickEvent;
        bool IsClosing { get; }
        uint TotalTicks { get; }
        string TickPhase { get; }
        void CloseSimulation();
    }

    public delegate void FirstTick();

    public delegate void TickSim(uint tickCount);
}