namespace TrailCommon
{
    public interface ITick : IMessage
    {
        string GetTUI();
        Randomizer Random { get; }
        event FirstTick FirstTickEvent;
        event Tick TickEvent;
        bool IsClosing { get; }
        uint TotalTicks { get; }
        string TickPhase { get; }
        void Destroy();
    }

    public delegate void FirstTick();

    public delegate void Tick(uint tickCount);
}