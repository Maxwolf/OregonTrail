namespace TrailCommon
{
    public interface ITick
    {
        Randomizer Random { get; }
        bool IsClosing { get; }
        void SystemTick();
        uint TotalTimerTicks { get; }
        uint TotalSystemTicks { get; }
        string TimerTickPhase { get; }
        event FirstTimerTick FirstTimerTickEvent;
        event TimerTick TimerTickEvent;
        event SystemTick SystemTickEvent;
        void Destroy();
    }

    public delegate void FirstTimerTick();

    public delegate void SystemTick(uint systemTickCount);

    public delegate void TimerTick(uint timerTickCount);
}