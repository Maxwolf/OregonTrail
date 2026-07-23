// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Vehicle
{
    /// <summary>
    ///     The pace at which you travel can change.
    /// </summary>
    public enum TravelPaceEnum
    {
        /// <summary>
        ///     8 hours a day, taking frequent rests. You take care not to get too tired.
        /// </summary>
        Steady = 1,

        /// <summary>
        ///     12 hours a day, stopping only when necessary. Finish each day feeling very tired.
        /// </summary>
        Strenuous = 2,

        /// <summary>
        ///     16 hours a day, you never rest and barley sleep. You are absolutely exhausted, and your health suffers.
        /// </summary>
        Grueling = 3
    }
}