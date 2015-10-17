namespace TrailCommon
{
    public interface ISimulation : ISimulationInitializer
    {
        Randomizer Random { get; }
        uint TotalTicks { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event TickSim TickEvent;
    }

    public delegate void TickSim(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();
}