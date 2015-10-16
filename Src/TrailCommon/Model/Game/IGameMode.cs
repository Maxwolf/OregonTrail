namespace TrailCommon
{
    public interface IGameMode
    {
        GameMode ModeType { get; }
        void TickMode();
        IVehicle Vehicle { get; }
    }
}