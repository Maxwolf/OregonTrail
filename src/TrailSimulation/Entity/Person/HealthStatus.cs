// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
{
    using System.ComponentModel;

    /// <summary>
    ///     Overall health indicator for all entities in the simulation, we do not track health as a numeric value but as a
    ///     enum state that has a roll chance of lowering to the lowest possible state over time.
    /// </summary>
    public enum HealthStatus
    {
        /// <summary>
        ///     Best and starting health of all entities in the simulation.
        /// </summary>
        Good = 500,

        /// <summary>
        ///     Some damage but still good, should reduce stress if possible.
        /// </summary>
        Fair = 400,

        /// <summary>
        ///     Damaged and under-performing, danger of failure.
        /// </summary>
        Poor = 300,

        /// <summary>
        ///     Severe damage, danger of complete failure of death imminent.
        /// </summary>
        [Description("Very Poor")]
        VeryPoor = 200,

        /// <summary>
        ///     Player is dead and no longer living. This means they also will no longer consume resources, or check of illnesses
        ///     or participate in point count at the end of the game if the player wins. If the person that died was the leader of
        ///     the party then the game ends entirely.
        /// </summary>
        Dead = 0
    }
}