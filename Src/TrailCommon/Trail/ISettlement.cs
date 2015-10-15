namespace TrailCommon
{
    public interface ISettlement
    {
        bool CanRest { get; }
        IStore Store { get; }
        void GoToStore();
    }
}
