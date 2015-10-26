namespace TrailCommon
{
    /// <summary>
    ///     Represents a store at interface level, allowing it's components to be accessed without knowing the actual object
    ///     type.
    /// </summary>
    public interface IStoreMode : IMode
    {
        /// <summary>
        ///     Holds all of the pending transactions the player would like to make with the store.
        /// </summary>
        StoreReceiptInfo StoreReceiptInfo { get; }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        ISettlementPoint CurrentSettlement { get; }

        /// <summary>
        ///     Name of the store is typically the location name with 'general store' added to the end of the name.
        /// </summary>
        string StoreName { get; }

        /// <summary>
        ///     Purchase an item from the store, it will take money from the player.
        /// </summary>
        /// <param name="item">Item they would like to purchase.</param>
        void BuyItems(IItem item);

        /// <summary>
        ///     Sell an item to the store, it will add money to the player.
        /// </summary>
        /// <param name="item">Item that will be removed from player and added to store.</param>
        void SellItem(IItem item);

        /// <summary>
        ///     Buy vehicle engine.
        /// </summary>
        void BuyOxen();

        /// <summary>
        ///     Buy food to feed party members with.
        /// </summary>
        void BuyFood();

        /// <summary>
        ///     Buy clothing to keep your party members warm in rough climates.
        /// </summary>
        void BuyClothing();

        /// <summary>
        ///     Buy bullets so your party leader can go into hunting mode and kill some animals for their meat.
        /// </summary>
        void BuyAmmunition();

        /// <summary>
        ///     Purchase spare parts for the vehicle like axles, tongues, and wheels.
        /// </summary>
        void BuySpareParts();

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        void LeaveStore();
    }
}