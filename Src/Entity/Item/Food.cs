namespace OregonTrail
{
    public class Food : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Food" /> class.
        /// </summary>
        public Food(Condition condition, string name, int cost, int quantity) : base(condition, name, cost, quantity)
        {
        }
    }
}