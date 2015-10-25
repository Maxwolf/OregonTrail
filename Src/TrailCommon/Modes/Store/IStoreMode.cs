using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IStoreMode : IMode
    {
        PointOfInterest CurrentSettlement { get; }
        string StoreName { get; }
        void BuyItems(IItem item);
        void SellItem(IItem item);
    }
}