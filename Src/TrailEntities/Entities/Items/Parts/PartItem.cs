namespace TrailEntities
{
    /// <summary>
    ///     Abstract class that is intended to be used for working with parts that are extra for the vehicle, once they are
    ///     attached we can monitor their status and individual repair status.
    /// </summary>
    public abstract class PartItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected PartItem(string name, uint quantity, uint weight, uint cost) : base(name, quantity, weight, cost)
        {
        }
    }
}