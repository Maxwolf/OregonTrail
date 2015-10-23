using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISimulation : ITick
    {
        IMode ActiveMode { get; }
        string ActiveModeName { get; }
        ReadOnlyCollection<IMode> Modes { get; }
        string ScreenBuffer { get; }
        string InputBuffer { get; }
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
        event ScreenBufferDirty ScreenBufferDirtyEvent;
        event InputBufferUpdated InputBufferUpdatedEvent;
        event ModeChanged ModeChangedEvent;
        void AddMode(ModeType modeType);
        void RemoveActiveMode();
        void RemoteLastCharOfInputBuffer();
        void ProcessInputBuffer();
        void SendKeyCharString(char keyChar);
        void TickTUI();
    }

    public delegate void ModeChanged(ModeType modeType);

    public delegate void EndGame();

    public delegate void ScreenBufferDirty(string tuiContent);

    public delegate void InputBufferUpdated(string inputBuffer, string addedKeycharString);

    public delegate void NewGame();
}