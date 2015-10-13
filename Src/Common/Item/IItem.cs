namespace OregonTrail.Common
{
    /// <summary>
    ///     Item that can be purchased by party, there is also a quantity associated with it. If zero that means there is no
    ///     more of that item even if it still exists in a collection.
    /// </summary>
    public interface IItem : IEntity
    {
        ItemCategory Description { get; }
        int Cost { get; }
        int Quantity { get; }
    }
}