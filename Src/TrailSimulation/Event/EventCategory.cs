namespace TrailSimulation.Event
{
    /// <summary>
    ///     Defines the different kinds of events the simulation supports, used for sorting and easy grabbing of events by type
    ///     for dice rolling purposes when picking random events.
    /// </summary>
    public enum EventCategory
    {
        /// <summary>
        ///     When something bad happens to vehicle.
        /// </summary>
        Vehicle = 1,

        /// <summary>
        ///     Something bad happens to oxen pulling the vehicle.
        /// </summary>
        Animal = 2,

        /// <summary>
        ///     Something bad happens to party member such as disease or illness.
        /// </summary>
        Person = 3,

        /// <summary>
        ///     Warnings about low food, medical problems, weather, etc.
        /// </summary>
        Warning = 4,

        /// <summary>
        ///     Used for displaying information about severe weather like blizzards and storms.
        /// </summary>
        Climate = 5,

        /// <summary>
        ///     Wild animals, Indians, wolves, riders, and various other critters and strangers that you can encounter.
        /// </summary>
        Attack = 6,

        /// <summary>
        ///     Crossing a river by diving directly into and hoping for the best. Easy to be washed away by current.
        /// </summary>
        RiverFord = 7,

        /// <summary>
        ///     Caulking the vehicle and attempting to float it across the river can result in flooding and or tipping over.
        /// </summary>
        RiverFloat = 8
    }
}