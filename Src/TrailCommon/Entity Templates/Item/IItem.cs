namespace TrailCommon
{
    /// <summary>
    ///     Item that can be purchased by party, there is also a quantity associated with it. If zero that means there is no
    ///     more of that item even if it still exists in a collection.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        ///     Display name of the item in question.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     How much the item weighs in pounds.
        /// </summary>
        uint Weight { get; }

        /// <summary>
        ///     How much the item costs to purchase.
        /// </summary>
        float Cost { get; }

        /// <summary>
        ///     How many of the item exist in this single instance.
        /// </summary>
        uint Quantity { get; }

        /// <summary>
        ///     Total weight of the items multiplied by the quantity of the stack.
        /// </summary>
        uint TotalWeight { get; }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        uint QuantityLimit { get; }
    }
}