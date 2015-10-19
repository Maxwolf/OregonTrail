namespace TrailCommon
{
    public interface IMode : IInvoker
    {
        void OnModeRemoved();
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        void TickMode();
    }
}