namespace TrailCommon
{
    public interface IMode
    {
        void OnModeRemoved();
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        void TickMode();
    }
}