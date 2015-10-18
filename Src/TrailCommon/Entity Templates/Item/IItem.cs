namespace TrailCommon
{
    /// <summary>
    ///     Item that can be purchased by party, there is also a quantity associated with it. If zero that means there is no
    ///     more of that item even if it still exists in a collection.
    /// </summary>
    public interface IItem
    {
        string Name { get; }
        uint Weight { get; }
        uint Cost { get; }
        uint Quantity { get; }
        void Buy(int amount);
        uint TotalWeight();
    }
}