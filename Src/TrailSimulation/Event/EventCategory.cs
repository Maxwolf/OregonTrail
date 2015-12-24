// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/02/2015@9:37 PM

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
        Vehicle,

        /// <summary>
        ///     Something bad happens to oxen pulling the vehicle.
        /// </summary>
        Animal,

        /// <summary>
        ///     Something bad happens to party member such as disease or injury.
        /// </summary>
        Person,

        /// <summary>
        ///     Used for displaying information about severe weather like blizzards and storms.
        /// </summary>
        Weather,

        /// <summary>
        ///     Wild animals, Indians, wolves, riders, and various other critters and strangers that you can encounter.
        /// </summary>
        Wild,

        /// <summary>
        ///     Crossing a river has many dangers regardless of transport mode. Flooding, capsizing, hitting rocks, etc.
        /// </summary>
        RiverCross
    }
}