namespace TrailEntities
{
    /// <summary>
    ///     Represents an animal that can provide a certain amount of food for zero cost and only weight.
    /// </summary>
    public abstract class Animal : Food
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected Animal() : base(0)
        {
        }
    }
}