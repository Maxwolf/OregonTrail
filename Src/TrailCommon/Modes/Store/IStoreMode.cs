using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IStoreMode : IMode<StoreCommands>
    {
        ReadOnlyCollection<IItem> StoreInventory { get; }
        string StoreName { get; }
        uint StoreBalance { get; }
        void BuyItems(IItem item);
        void SellItem(IItem item);
    }
}