namespace TrailCommon
{
    public interface IMode : IMessage
    {
        ModeType Mode { get; }
        IGameSimulation Game { get; }
        string GetTUI();
        void TickMode();
    }
}