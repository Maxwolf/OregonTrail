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
    }

    public delegate void ModeChanged(SimulationMode mode);

    public delegate void TickSim(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();
}