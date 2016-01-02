// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System.ComponentModel;

    /// <summary>
    ///     Defines a rating the player can get based on the number of points they receive during the entire course of the
    ///     game. At the end after tabulation this enum is assigned as an overall representation of the scoring level.
    /// </summary>
    public enum Performance
    {
        /// <summary>
        ///     Easy
        /// </summary>
        Greenhorn = 1,

        /// <summary>
        ///     Medium
        /// </summary>
        Adventurer = 2,

        /// <summary>
        ///     Hard
        /// </summary>
        [Description("Trail Guide")]
        TrailGuide = 3
    }
}