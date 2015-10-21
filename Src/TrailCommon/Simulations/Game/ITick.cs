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
        void Create();
        void Destroy();
        void SendCommand(string returnedLine);
    }

    public delegate void FirstTick();

    public delegate void TickSim(uint tickCount);
}