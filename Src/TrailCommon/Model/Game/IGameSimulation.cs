namespace TrailCommon
{
    public interface IGameSimulation : ISimulation
    {
        void TakeTurn();
        void Hunt();
        void Rest();
        void Trade();
        ITimeSimulation Time { get; }
        IClimateSimulation Climate { get; }
    }
}
