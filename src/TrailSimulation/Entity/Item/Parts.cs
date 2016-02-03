// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
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
            get { return new SimItem(Entities.Animal, "Oxen", "oxen", "ox", 20, 20, 0, 1, 0, 4); }
        }

        /// <summary>
        ///     Required to keep the vehicle moving if this part is broken it must be replaced before the player can
        ///     continue their journey.
        /// </summary>
        public static SimItem Axle
        {
            get { return new SimItem(Entities.Axle, "Vehicle Axle", "axles", "axle", 3, 10, 0, 1, 0, 2); }
        }

        /// <summary>
        ///     Required to keep the vehicle running, if the tongue breaks then the player will have to fix or replace it before
        ///     they can continue on the journey again.
        /// </summary>
        public static SimItem Tongue
        {
            get { return new SimItem(Entities.Tongue, "Vehicle Tongue", "tongues", "tongue", 3, 10, 0, 1, 0, 2); }
        }

        /// <summary>
        ///     Required to keep the vehicle moving down the path, if any of the wheel parts break they must be replaced before the
        ///     journey can continue.
        /// </summary>
        public static SimItem Wheel
        {
            get { return new SimItem(Entities.Wheel, "Vehicle Wheel", "wheels", "wheel", 3, 10, 0, 1, 0, 2); }
        }
    }
}