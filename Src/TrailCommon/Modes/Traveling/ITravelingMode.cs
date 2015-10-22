namespace TrailCommon
{
    public interface ITravelingMode : IMode
    {
        void Hunt();
        void Rest();
        void Trade();
    }
}