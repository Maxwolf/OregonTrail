namespace TrailCommon
{
    public interface IGameMode
    {
        string Name { get; }

        void ModeChange();
        event ModeChanged ModeChangedEvent;

        IVehicle Vehicle { get; }
    }

    public delegate void ModeChanged();
}