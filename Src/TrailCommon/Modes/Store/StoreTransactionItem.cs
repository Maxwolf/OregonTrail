namespace TrailCommon
{
    /// <summary>
    ///     Defines a class that is capable of storing a single transaction the player would like to make with a store.
    /// </summary>
    public sealed class StoreTransactionItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.StoreTransactionItem" /> class.
        /// </summary>
        public StoreTransactionItem(uint quantity, Item item)
        {
            Quantity = quantity;
            Item = item;
        }

        /// <summary>
        ///     Total number of the items the player is going to be taking.
        /// </summary>
        public uint Quantity { get; }

        /// <summary>
        ///     Reference to the actual item the player is going to be taking into their vehicle inventory, or maybe the store is
        ///     going to be taking it from the player, or it could also be for a trade!
        /// </summary>
        public Item Item { get; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return (Quantity*Item.Cost).ToString("C2");
        }
    }
}