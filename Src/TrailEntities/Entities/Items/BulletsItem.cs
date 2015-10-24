namespace TrailEntities
{
    public sealed class BulletsItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public BulletsItem(string name, uint quantity, uint weight, uint cost) : base(name, quantity, weight, cost)
        {
        }
    }
}