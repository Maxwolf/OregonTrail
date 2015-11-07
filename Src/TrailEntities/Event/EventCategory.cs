namespace TrailEntities
{
    /// <summary>
    ///     Defines the different kinds of events the simulation supports, used for sorting and easy grabbing of events by type
    ///     for dice rolling purposes when picking random events.
    /// </summary>
    public enum EventCategory
    {
        /// <summary>
        ///     Reserved for default event initialization.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     When something bad happens to vehicle or party members.
        /// </summary>
        Accident = 1,

        /// <summary>
        ///     Something bad happens to oxen pulling the vehicle.
        /// </summary>
        Animal = 2,

        /// <summary>
        ///     Something bad happens to party member such as disease or illness.
        /// </summary>
        Medical = 3,

        /// <summary>
        ///     Warnings about low food, medical problems, weather, etc.
        /// </summary>
        Warning = 4,

        /// <summary>
        ///     Used for displaying information about severe weather like blizzards and storms.
        /// </summary>
        Weather = 5,

        /// <summary>
        ///     Wild animals, Indians, wolves, riders, and various other critters and strangers that you can encounter.
        /// </summary>
        Wild = 6
    }
}