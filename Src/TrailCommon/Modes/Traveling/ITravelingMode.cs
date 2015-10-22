namespace TrailCommon
{
    public interface ITravelingMode : IMode<TravelCommands>
    {
        void Hunt();
        void Rest();
        void Trade();
    }
}