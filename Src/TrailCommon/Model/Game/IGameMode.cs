namespace TrailCommon
{
    public interface IGameMode
    {
        SimulationMode Mode { get; }
        void TickMode();
        IVehicle Vehicle { get; }
    }
}