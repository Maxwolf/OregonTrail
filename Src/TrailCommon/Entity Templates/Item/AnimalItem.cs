namespace TrailCommon
{
    /// <summary>
    ///     Represents an animal that can provide a certain amount of food for zero cost and only weight.
    /// </summary>
    public abstract class AnimalItem : FoodItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Item" /> class.
        /// </summary>
        protected AnimalItem() : base(0)
        {
        }
    }
}