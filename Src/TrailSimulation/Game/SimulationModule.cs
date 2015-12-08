using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Since the view for game modes is separated from the actual logic being performed we need a logical way to know what
    ///     view to attach on the view. This enum serves that purpose, it is required to add any new game modes the simulation
    ///     needs to know about to this file.
    /// </summary>
    public enum SimulationModule
    {
        /// <summary>
        ///     Primary game Windows used for advancing simulation down the trail.
        /// </summary>
        [Window(typeof (Travel))]
        Travel,

        /// <summary>
        ///     Allows the configuration of party names, player profession, and purchasing initial items for trip.
        /// </summary>
        [Window(typeof (MainMenu))]
        MainMenu,

        /// <summary>
        ///     Randomizer event Windows is attached by the event director which then listens for the event it will throw at it over
        ///     event delegate the random event Windows will subscribe to.
        /// </summary>
        [Window(typeof (RandomEvent))]
        RandomEvent
    }
}