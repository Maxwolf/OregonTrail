using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISimulation : ITick
    {
        IServerPipe Server { get; }
        uint TotalClients { get; }
        IMode ActiveMode { get; }
        string ActiveModeName { get; }
        ReadOnlyCollection<IMode> Modes { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event ModeChanged ModeChangedEvent;
        void AddMode(ModeType mode);
        void RemoveMode(ModeType mode);
        void StartGame();
    }

    public delegate void ModeChanged(ModeType mode);

    public delegate void EndGame();

    public delegate void NewGame();
}