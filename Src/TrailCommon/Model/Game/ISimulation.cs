namespace TrailCommon
{
    public interface ISimulation : ISimulationInitializer
    {
        uint TotalTicks { get; }
        void SetMode(ITrailMode mode);
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event ModeChanged ModeChangedEvent;
        event TickTimeHandler TickEvent;
    }

    public delegate void TickTimeHandler(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();

    public delegate void ModeChanged(TrailModeType modeType);
}