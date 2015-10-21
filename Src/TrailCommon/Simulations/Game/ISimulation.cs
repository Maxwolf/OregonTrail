using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISimulation : ITick
    {
        IMode ActiveMode { get; }
        string ActiveModeName { get; }
        ReadOnlyCollection<IMode> Modes { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event ScreenBufferDirty ScreenBufferDirtyEvent;
        event ModeChanged ModeChangedEvent;
        void AddMode(ModeType mode);
        void RemoveActiveMode();
        string ScreenBuffer { get; }
        string InputBuffer { get; }
        void RemoteLastCharOfInputBuffer();
        void ProcessInputBuffer();
        void SendKeyCharString(char keyChar);
        void TickTUI();
    }

    public delegate void ModeChanged(ModeType mode);

    public delegate void EndGame();

    public delegate void ScreenBufferDirty(string tuiContent);

    public delegate void NewGame();
}