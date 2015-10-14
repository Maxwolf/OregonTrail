using System.Collections.Generic;

namespace TrailCommon
{
    public interface IStore
    {
        IVehicle Vehicle { get; }
        SortedSet<IItem> Inventory { get; }
        uint Balance { get; }
        void BuyItems(IItem item);
        void SellItem(IItem item);
    }
}