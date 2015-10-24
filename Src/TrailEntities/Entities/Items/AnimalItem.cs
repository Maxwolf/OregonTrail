namespace TrailEntities
{
    /// <summary>
    ///     Represents an animal that can provide a certain amount of food for zero cost and only weight.
    /// </summary>
    public abstract class AnimalItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.AnimalItem" /> class.
        /// </summary>
        protected AnimalItem() : base(0)
        {
        }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        public override uint Quantity
        {
            get { return 1; }
        }
    }
}