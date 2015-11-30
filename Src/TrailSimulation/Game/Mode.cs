using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Since the view for game modes is separated from the actual logic being performed we need a logical way to know what
    ///     view to attach on the view. This enum serves that purpose, it is required to add any new game modes the simulation
    ///     needs to know about to this file.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        ///     Primary game mode used for advancing simulation down the trail.
        /// </summary>
        [SimulationMode(typeof (TravelMode))]
        Travel,

        /// <summary>
        ///     Lets the player hunt for food to bring back to the vehicle.
        /// </summary>
        [SimulationMode(typeof (HuntingMode))]
        Hunt,

        /// <summary>
        ///     Allows the configuration of party names, player profession, and purchasing initial items for trip.
        /// </summary>
        [SimulationMode(typeof (MainMenuMode))]
        MainMenu,

        /// <summary>
        ///     Shows final point count, resets simulation data, asks if user wants to return to main menu or close the
        ///     application.
        /// </summary>
        [SimulationMode(typeof (EndGameMode))]
        EndGame,

        /// <summary>
        ///     Facilitates purchasing items from a list, prices can change per store as there is no central lookup for this
        ///     information.
        /// </summary>
        [SimulationMode(typeof (StoreMode))]
        Store,

        /// <summary>
        ///     Facilitates trading items with a fake AI vehicle, a list is created and values randomly selected from it for
        ///     possible trades.
        /// </summary>
        [SimulationMode(typeof (TradingMode))]
        Trade,

        /// <summary>
        ///     Allows the player to reset top ten high scores, remove saved games, remove tombstone messages, etc.
        /// </summary>
        [SimulationMode(typeof (OptionsMode))]
        Options,

        /// <summary>
        ///     Random event mode is attached by the event director which then listens for the event it will throw at it over
        ///     event delegate the random event mode will subscribe to.
        /// </summary>
        [SimulationMode(typeof (RandomEventMode))]
        RandomEvent
    }
}