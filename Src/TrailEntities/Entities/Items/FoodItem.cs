namespace TrailEntities
{
    public abstract class FoodItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected FoodItem(uint cost) : base(cost)
        {
        }
    }
}