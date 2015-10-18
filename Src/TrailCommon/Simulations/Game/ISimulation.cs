using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISimulation
    {
        ISenderPipe SendPipe { get; }
        IReceiverPipe RecievePipe { get; }
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
        event ModeChanged ModeChangedEvent;
        void AddMode(ModeType mode);
        void RemoveMode(ModeType mode);
        void StartGame();
        void CloseSimulation();
    }

    public delegate void ModeChanged(ModeType mode);

    public delegate void TickSim(uint tickCount);

    public delegate void EndGame();

    public delegate void NewGame();
}