using System.ComponentModel;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Overall health indicator for all entities in the simulation, we do not track health as a numeric value but as a
    ///     enum state that has a roll chance of lowering to the lowest possible state over time.
    /// </summary>
    public enum RepairLevel
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
        VeryPoor = 200
    }
}