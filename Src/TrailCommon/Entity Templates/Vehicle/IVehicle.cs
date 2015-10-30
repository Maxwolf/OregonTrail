using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Defines a basic vessel to carry parties of people. It's presence is defined by a current location, the amount of
    ///     distance it has traveled, and the parts that make up the entire vehicle (each with it's own health).
    /// </summary>
    public interface IVehicle
    {
        ReadOnlyCollection<Item> Inventory { get; }
        float Balance { get; }
        ReadOnlyCollection<IPerson> People { get; }
        RationLevel Ration { get; }
        TravelPace Pace { get; }
        RepairStatus RepairStatus { get; }
        uint Odometer { get; }
        void AddPerson(IPerson person);
        void AddItem(Item item);
        void BuyItem(StoreTransactionItem transaction);
        void SellItem(StoreTransactionItem transaction);
        void UpdateVehicle();
        void ResetVehicle(uint startingMonies);
    }
}