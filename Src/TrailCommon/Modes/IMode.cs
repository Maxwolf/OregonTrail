namespace TrailCommon
{
    public interface IMode
    {
        void OnModeRemoved();
        string GetTUI();
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        void TickMode();
    }
}