namespace TrailEntities
{
    /// <summary>
    ///     Required to keep the vehicle running, if the tongue breaks then the player will have to fix or replace it before
    ///     they can continue on the journey again.
    /// </summary>
    public sealed class VehicleTongueItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.VehicleTongueItem" /> class.
        /// </summary>
        public VehicleTongueItem(float cost) : base(cost, 1)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Vehicle Tongue"; }
        }
    }
}