namespace TrailCommon
{
    public interface IMode
    {
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        void TickMode();
    }
}