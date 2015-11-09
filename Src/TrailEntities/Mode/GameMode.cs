using TrailEntities.Game;
using TrailEntities.Game.EndGame;
using TrailEntities.Game.ForkInRoad;
using TrailEntities.Game.Hunting;
using TrailEntities.Game.MainMenu;
using TrailEntities.Game.Options;
using TrailEntities.Game.RandomEvent;
using TrailEntities.Game.RiverCross;
using TrailEntities.Game.Store;
using TrailEntities.Game.Trade;
using TrailEntities.Game.Travel;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Since the view for game GameMode is separated from the actual logic being performed we need a logical way to know what
    ///     view to attach on the view. This enum serves that purpose, it is required to add any new game GameMode the simulation
    ///     needs to know about to this enumeration along with game gameMode attribute to define required classes for game gameMode.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        ///     Primary game gameMode used for advancing simulation down the trail.
        /// </summary>
        [GameMode(typeof(TravelGameMode), typeof(TravelCommands), typeof(TravelInfo))]
        Travel,

        /// <summary>
        ///     Forces the player to make a decision about where to go next on the trail.
        /// </summary>
        [GameMode(typeof(ForkInRoadGameMode), typeof(ForkInRoadCommands), typeof(ForkInRoadInfo))]
        ForkInRoad,

        /// <summary>
        ///     Lets the player hunt for food to bring back to the vehicle.
        /// </summary>
        [GameMode(typeof(HuntingGameMode), typeof(HuntingCommands), typeof(HuntingInfo))]
        Hunt,

        /// <summary>
        ///     Allows the configuration of party names, player profession, and purchasing initial items for trip.
        /// </summary>
        [GameMode(typeof(MainMenuGameMode), typeof(MainMenuCommands), typeof(MainMenuInfo))]
        MainMenu,

        /// <summary>
        ///     Shows final point count, resets simulation data, asks if user wants to return to main menu or close the
        ///     application.
        /// </summary>
        [GameMode(typeof(EndGameMode), typeof(EndGameCommands), typeof(EndGameInfo))]
        EndGame,

        /// <summary>
        ///     Forces the player to make a choice about how to cross the river, they can ford the river, caulk their wagon and
        ///     float, or pay to take a ferry across.
        /// </summary>
        [GameMode(typeof(RiverCrossGameMode), typeof(RiverCrossCommands), typeof(RiverCrossInfo))]
        RiverCrossing,

        /// <summary>
        ///     Facilitates purchasing items from a list, prices can change per store as there is no central lookup for this
        ///     information.
        /// </summary>
        [GameMode(typeof(StoreGameMode), typeof(StoreCommands), typeof(StoreInfo))]
        Store,

        /// <summary>
        ///     Facilitates trading items with a fake AI vehicle, a list is created and values randomly selected from it for
        ///     possible trades.
        /// </summary>
        [GameMode(typeof(TradingGameMode), typeof(TradingCommands), typeof(TradeInfo))]
        Trade,

        /// <summary>
        ///     Allows the player to reset top ten high scores, remove saved games, remove tombstone messages, etc.
        /// </summary>
        [GameMode(typeof(OptionsGameMode), typeof(OptionCommands), typeof(OptionInfo))]
        Options,

        /// <summary>
        ///     Random event gameMode is attached by the event director which then listens for the event it will throw at it over event
        ///     delegate the random event gameMode will subscribe to.
        /// </summary>
        [GameMode(typeof(RandomEventGameMode), typeof(RandomEventCommands), typeof(RandomEventInfo))]
        RandomEvent
    }
}