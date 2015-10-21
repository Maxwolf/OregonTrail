namespace TrailCommon
{
    public interface IMode : IMessage
    {
        string GetTUI();
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        void TickMode();
    }
}