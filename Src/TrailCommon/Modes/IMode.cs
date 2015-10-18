namespace TrailCommon
{
    public interface IMode
    {
        ModeType Mode { get; }
        IGameServer Game { get; }
        void TickMode();
    }
}