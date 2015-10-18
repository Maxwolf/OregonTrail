namespace TrailCommon
{
    public interface IGameServer : ISimulation
    {
        ITimeSimulation Time { get; }
        IClimateSimulation Climate { get; }
        IVehicle Vehicle { get; }
        uint TotalTurns { get; }
        void TakeTurn();
        void OnDestroy();
    }
}