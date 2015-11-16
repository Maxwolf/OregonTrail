using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Since the view for game modes is separated from the actual logic being performed we need a logical way to know what
    ///     view to attach on the view. This enum serves that purpose, it is required to add any new game modes the simulation
    ///     needs to know about to this file.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        ///     Primary game mode used for advancing simulation down the trail.
        /// </summary>
        [GameMode(typeof (TravelMode))]
        Travel,

        /// <summary>
        ///     Forces the player to make a decision about where to go next on the trail.
        /// </summary>
        [GameMode(typeof (ForkInRoadMode))]
        ForkInRoad,

        /// <summary>
        ///     Lets the player hunt for food to bring back to the vehicle.
        /// </summary>
        [GameMode(typeof (HuntingMode))]
        Hunt,

        /// <summary>
        ///     Allows the configuration of party names, player profession, and purchasing initial items for trip.
        /// </summary>
        [GameMode(typeof (MainMenuMode))]
        MainMenu,

        /// <summary>
        ///     Shows final point count, resets simulation data, asks if user wants to return to main menu or close the
        ///     application.
        /// </summary>
        [GameMode(typeof (EndGameMode))]
        EndGame,

        /// <summary>
        ///     Forces the player to make a choice about how to cross the river, they can ford the river, caulk their wagon and
        ///     float, or pay to take a ferry across.
        /// </summary>
        [GameMode(typeof (RiverCrossMode))]
        RiverCrossing,

        /// <summary>
        ///     Facilitates purchasing items from a list, prices can change per store as there is no central lookup for this
        ///     information.
        /// </summary>
        [GameMode(typeof (StoreMode))]
        Store,

        /// <summary>
        ///     Facilitates trading items with a fake AI vehicle, a list is created and values randomly selected from it for
        ///     possible trades.
        /// </summary>
        [GameMode(typeof (TradingMode))]
        Trade,

        /// <summary>
        ///     Allows the player to reset top ten high scores, remove saved games, remove tombstone messages, etc.
        /// </summary>
        [GameMode(typeof (OptionsMode))]
        Options,

        /// <summary>
        ///     Random event mode is attached by the event director which then listens for the event it will throw at it over event
        ///     delegate the random event mode will subscribe to.
        /// </summary>
        [GameMode(typeof (RandomEventMode))]
        RandomEvent
    }
}