// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.TrailSimulation.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     Defines all of the different types of river crossings, there are several different configurations. One of them is
    ///     to only be able to fork and caulk the vehicle over the river, the other is to be able to take a ferry, another is
    ///     to ask an Indian for assistance. If any other options need to exist they should be added here so locations can
    ///     reference them and the river form deal with the appropriately.
    /// </summary>
    public enum RiverOption
    {
        /// <summary>
        ///     Default river option, will throw an exception if this is used. Locations default to using fork and ford. This value
        ///     exists for initialization purposes only.
        /// </summary>
        None = 0,

        /// <summary>
        ///     No special assistance with crossing river, player must ford into it or caulk and float their vehicle across it.
        /// </summary>
        FloatAndFord = 1,

        /// <summary>
        ///     Provides access to a ferry alongside the fork and ferry abilities, cost the player monies and time to use it. Ferry
        ///     operator may force the player to wait several days in line for the ferry before they can use it.
        /// </summary>
        FerryOperator = 2,

        /// <summary>
        ///     Offers up an Indian guide that will help you float your vehicle across the river. However, unlike the ferry
        ///     operator he works in exchange for sets of clothing which are randomized and then multiplied by the number of
        ///     animals the player has killed. This means that if the player is careful to not over hunt they will get much better
        ///     deals from the Indian on the river crossing.
        /// </summary>
        IndianGuide = 3
    }
}