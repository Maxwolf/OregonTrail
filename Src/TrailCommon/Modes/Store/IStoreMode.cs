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
        StoreInfo StoreInfo { get; }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        IPoint CurrentPoint { get; }

        /// <summary>
        ///     Purchase an item from the store, it will take money from the player.
        /// </summary>
        void BuyItems(StoreTransactionItem transaction);

        /// <summary>
        ///     Sell an item to the store, it will add money to the player.
        /// </summary>
        void SellItem(StoreTransactionItem transaction);

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
        ///     Purchase spare wheels for the vehicle.
        /// </summary>
        void BuySpareWheels();

        /// <summary>
        ///     Purchase spare axles for the vehicle.
        /// </summary>
        void BuySpareAxles();

        /// <summary>
        ///     Purchase spare tongues for the vehicle.
        /// </summary>
        void BuySpareTongues();

        /// <summary>
        ///     Attaches a game mode state what will show the player some basic information about what the various items mean and
        ///     what their purpose is in the simulation.
        /// </summary>
        void StoreAdvice();

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        void LeaveStore();
    }
}