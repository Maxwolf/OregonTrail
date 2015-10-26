using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ISettlementPoint
    {
        string Name { get; }
        bool CanRest { get; }
        bool HasStore { get; }
        ReadOnlyCollection<IItem> StoreItems { get; }
        float StoreBalance { get; }
        void GoToStore();
    }
}