namespace TrailEntities.Entity
{
    /// <summary>
    ///     Defines all the possible types of items, used for sorting and quickly being able to determine type when iterating
    ///     over them in a list.
    /// </summary>
    public enum SimEntity
    {
        /// <summary>
        ///     Defines the vessel in which the party members, their inventory, monies, hopes and dreams, and everything else
        ///     resides. The purpose of this enum value is so we can treat the entity properly and give it a type.
        /// </summary>
        Vehicle,


        /// <summary>
        ///     Represents a given occupant in the vehicle, this is used mostly to separate the player entities from vehicle and
        ///     ensure the game never confuses them for being items.
        /// </summary>
        Person,

        /// <summary>
        ///     Represents how much monies the player decided to spend on Oxen when purchasing initial items for the journey on the
        ///     trail. The purpose for this is to offer up a clear distinction between a part of a vehicle and something that is
        ///     helping to pull it along. In a manner of speaking you could treat the animal like the fuel in a vehicle, if it
        ///     drains completely it will be considered broken and can no longer assist in pulling the vehicle along the trail.
        /// </summary>
        Animal,

        /// <summary>
        ///     Part on vehicle that must be kept track of, if it breaks the user will have to use another one to fix it.
        /// </summary>
        Wheel,

        /// <summary>
        ///     Tongue for the vehicle which connects it to animals that are pulling (typically this is oxen).
        /// </summary>
        Tongue,

        /// <summary>
        ///     Axle that connects the vehicle to wheels, if this part is broken it must be repaired or the total possible mileage
        ///     for the current two-week block the simulation is running.
        /// </summary>
        Axle,

        /// <summary>
        ///     Used in hunting and for killing wild animals, can be purchased from stores and also traded with other travelers on
        ///     the trail.
        /// </summary>
        Ammo,

        /// <summary>
        ///     Clothing is used by the party members to keep them warm when the climate simulation lowers the ambient temperature.
        ///     Without proper shielding against the elements the risk for disease and critical failures increase exponentially.
        /// </summary>
        Clothes,

        /// <summary>
        ///     Food from hunting or stores. Represented in pounds of meat. Can typically take back only 250 pounds to vehicle from
        ///     hunting. Consumed by the party members at the end of each day of the simulation. Depending on ration level the
        ///     amount of food in pounds eaten each day can vary.
        /// </summary>
        Food,

        /// <summary>
        ///     Represents paper currency which can be exchanged for goods at store. The game makes no attempt at money delineation
        ///     outside of quantity of single dollars.
        /// </summary>
        Cash
    }
}