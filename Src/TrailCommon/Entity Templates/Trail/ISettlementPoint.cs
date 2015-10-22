namespace TrailCommon
{
    public interface ISettlementPoint
    {
        bool CanRest { get; }
        IStoreMode StoreMode { get; }
        void GoToStore();
    }
}