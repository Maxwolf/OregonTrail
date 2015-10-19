using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISimulation
    {
        SimulationType SimulationType { get; }
        IServerPipe Server { get; }
        IClientPipe Client { get; }
        uint TotalClients { get; }
        IMode ActiveMode { get; }
        string ActiveModeName { get; }
        ReadOnlyCollection<IMode> Modes { get; }
        Randomizer Random { get; }
        uint TotalTicks { get; }
        string TickPhase { get; }
        bool IsClosing { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event TickSim TickEvent;
        event FirstTick FirstTickEvent;
        event ModeChanged ModeChangedEvent;
        void AddMode(ModeType mode);
        void RemoveMode(ModeType mode);
        void StartGame();
        void CloseSimulation();
    }

    public delegate void ModeChanged(ModeType mode);

    public delegate void TickSim(uint tickCount);

    public delegate void FirstTick();

    public delegate void EndGame();

    public delegate void NewGame();
}