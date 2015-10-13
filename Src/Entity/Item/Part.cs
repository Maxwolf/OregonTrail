namespace OregonTrail
{
    public class Part : ItemBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Part" /> class.
        /// </summary>
        public Part(Condition condition, string name, int cost, int quantity) : base(condition, name, cost, quantity)
        {
        }
    }
}