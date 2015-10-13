namespace OregonTrail
{
    /// <summary>
    ///     Item that can be purchased by party, there is also a quantity assocaited with it. If zero that means there is no
    ///     more of that item even if it still exists in a collection.
    /// </summary>
    public interface IItem : IEntity
    {
        int Cost { get; set; }
        int Quantity { get; set; }
    }
}