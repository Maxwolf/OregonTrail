namespace TrailCommon
{
    public interface IGameSimulation : ISimulation
    {
        ITimeSimulation Time { get; }
        IClimateSimulation Climate { get; }
        void TakeTurn();
        void Hunt();
        void Rest();
        void Trade();
    }
}