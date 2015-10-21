namespace TrailCommon
{
    public interface ITick
    {
        Randomizer Random { get; }
        bool IsClosing { get; }
        uint TotalTimerTicks { get; }
        uint TotalSystemTicks { get; }
        string TimerTickPhase { get; }
        void SystemTick();
        event FirstTimerTick FirstTimerTickEvent;
        event TimerTick TimerTickEvent;
        event SystemTick SystemTickEvent;
        void Destroy();
    }

    public delegate void FirstTimerTick();

    public delegate void SystemTick(uint systemTickCount);

    public delegate void TimerTick(uint timerTickCount);
}