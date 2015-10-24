namespace TrailEntities
{
    public sealed class VehicleAxleItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public VehicleAxleItem(string name, uint quantity, uint weight, uint cost) : base(name, quantity, weight, cost)
        {
        }
    }
}