using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IStoreMode : IMode
    {
        ReadOnlyCollection<IItem> StoreInventory { get; }
        string StoreName { get; }
        float StoreBalance { get; }
        void BuyItems(IItem item);
        void SellItem(IItem item);
    }
}