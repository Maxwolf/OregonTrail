using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Since the forms for game windows are separated from the actual logic being performed we need a logical way to know
    ///     what form to attach on the window. This enum serves that purpose, it is required to add any new game modes the
    ///     simulation needs to know about to this file.
    /// </summary>
    public enum GameWindow
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
        ///     Random event window is attached by the event director which then listens for the event it will throw at it over
        ///     event delegate the random event window will subscribe to.
        /// </summary>
        [Window(typeof (RandomEvent))]
        RandomEvent,

        /// <summary>
        ///     Displays the name of a previous player whom traveled the trail and died at a given mile marker. There is also an
        ///     optional epitaph that can be displayed. These tombstones are saved per trail, and can be reset from main menu.
        /// </summary>
        [Window(typeof (TombstoneWindow))]
        Tombstone
    }
}