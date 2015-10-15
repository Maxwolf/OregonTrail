using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IStore
    {
        IVehicle Vehicle { get; }
        ReadOnlyCollection<IItem> Inventory { get; }
        string StoreName { get; }
        uint StoreBalance { get; }
        void BuyItems(IItem item);
        void SellItem(IItem item);
    }
}