namespace TrailEntities.Entity
{
    /// <summary>
    ///     Defines a bunch of items that are used as parts in the vehicle.
    /// </summary>
    public static class Parts
    {
        /// <summary>
        ///     Zero weight animal that is attached to the vehicle but not actually 'inside' of it, but is still in the list of
        ///     inventory items that define the vehicle the player and his party is making the journey in.
        /// </summary>
        public static SimItem Oxen
        {
            get { return new SimItem(SimEntity.Animal, "Oxen", "oxen", "ox", 20, 20, 0); }
        }

        /// <summary>
        ///     Required to keep the vehicle moving if this part is broken it must be replaced before the player can
        ///     continue their journey.
        /// </summary>
        public static SimItem Axle
        {
            get { return new SimItem(SimEntity.Axle, "Vehicle Axle", "axles", "axle", 3, 10, 0); }
        }

        /// <summary>
        ///     Required to keep the vehicle running, if the tongue breaks then the player will have to fix or replace it before
        ///     they can continue on the journey again.
        /// </summary>
        public static SimItem Tongue
        {
            get { return new SimItem(SimEntity.Tongue, "Vehicle Tongue", "tongues", "tongue", 3, 10, 0); }
        }

        /// <summary>
        ///     Required to keep the vehicle moving down the path, if any of the wheel parts break they must be replaced before the
        ///     journey can continue.
        /// </summary>
        public static SimItem Wheel
        {
            get { return new SimItem(SimEntity.Wheel, "Vehicle Wheel", "wheels", "wheel", 3, 10, 0); }
        }
    }
}