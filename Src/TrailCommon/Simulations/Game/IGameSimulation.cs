namespace TrailCommon
{
    public interface IGameSimulation : ISimulation
    {
        ITimeSimulation Time { get; }
        IClimateSimulation Climate { get; }
        IEventSimulation Director { get; }
        IVehicle Vehicle { get; }
        uint TotalTurns { get; }
        void TakeTurn();
    }
}