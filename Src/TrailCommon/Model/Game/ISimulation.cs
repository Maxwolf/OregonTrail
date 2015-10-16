using System;

namespace TrailCommon
{
    public interface ISimulation : ISimulationInitializer
    {
        Randomizer Random { get; }
        uint TotalTicks { get; }
        void SetMode(IGameMode mode);
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event ModeChanged ModeChangedEvent;
        event TickSim TickEvent;
    }

    public delegate void TickSim(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();

    public delegate void ModeChanged(GameMode mode);
}