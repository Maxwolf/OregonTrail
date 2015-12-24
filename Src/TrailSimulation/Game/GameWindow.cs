// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/23/2015@4:34 PM

namespace TrailSimulation.Game
{
    using Core;

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
        [Window(typeof (Graveyard))]
        Graveyard,

        /// <summary>
        ///     Controls the process of ending the current game simulation depending on if the player won or lost. This window can
        ///     be attached at any point by any other window, or form in order to facilitate the game being able to trigger a game
        ///     over scenario no matter what is happening.
        /// </summary>
        [Window(typeof (GameOver))]
        GameOver
    }
}