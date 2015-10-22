namespace TrailCommon
{
    public interface ISettlementPoint
    {
        bool CanRest { get; }
        IStore Store { get; }
        void GoToStore();
    }
}