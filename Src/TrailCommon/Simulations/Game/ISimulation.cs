namespace TrailCommon
{
    public interface ISimulation : ISimulationInitializer
    {
        Randomizer Random { get; }
        uint TotalTicks { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event TickSim TickEvent;
        event ModeChanged ModeChangedEvent;
        void SetMode(ModeType mode);
    }

    public delegate void ModeChanged(ModeType mode);

    public delegate void TickSim(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();
}