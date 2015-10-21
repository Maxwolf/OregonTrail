namespace TrailCommon
{
    public interface ITick : IMessage
    {
        Randomizer Random { get; }
        bool IsClosing { get; }
        uint TotalTicks { get; }
        string TickPhase { get; }
        string GetTUI();
        event FirstTick FirstTickEvent;
        event Tick TickEvent;
        void Destroy();
    }

    public delegate void FirstTick();

    public delegate void Tick(uint tickCount);
}