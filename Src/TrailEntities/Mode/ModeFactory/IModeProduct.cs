using System;
using TrailEntities.Simulation.Trail;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Defines basic interface for a game mode that can be attached to running simulation.
    /// </summary>
    public interface IModeProduct
    {
        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game gameMode might
        ///     need
        ///     to at the top of menu selections.
        /// </summary>
        string MenuHeader { get; set; }

        /// <summary>
        ///     Similar to the header this will define some text that should go below the menu selection but before the user input
        ///     field.
        /// </summary>
        string MenuFooter { get; set; }

        /// <summary>
        ///     Determines if the command names for the particular action should be printed out alongside the number the user can
        ///     press to control that particular enum.
        /// </summary>
        bool ShowCommandNamesInMenu { get; }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        Location CurrentPoint { get; }

        /// <summary>
        ///     Determines if the game gameMode should not be ticked if it is active but instead removed. The gameMode when set to
        ///     being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        bool ShouldRemoveMode { get; }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        GameMode ModeType { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Holds the current state which this gameMode is in, a gameMode will cycle through available states until it is
        ///     finished and
        ///     then detach.
        /// </summary>
        StateProduct CurrentState { get; }

        /// <summary>
        ///     User data objects for game modes and any attached states on them.
        /// </summary>
        IModeInfo UserData { get; }

        /// <summary>
        ///     Parses the type of state passed in as parameter and creates the state by manually calling the constructor on it and
        ///     passing instances of parent game gameMode and user info object.
        /// </summary>
        /// <param name="state">EventType of the gameMode state class which should be attached to this game gameMode.</param>
        void AddState(Type state);

        /// <summary>
        ///     Clears the current state in this simulation game gameMode, resetting it back to default of empty revealing command
        ///     menu.
        /// </summary>
        void RemoveState();

        /// <summary>
        ///     Sets the flag for this game gameMode to be removed the next time it is ticked by the simulation.
        /// </summary>
        void SetShouldRemoveMode();

        /// <summary>
        ///     Fired by simulation when it wants to request latest text user interface data for the game gameMode, this is used to
        ///     display to user console specific information about what the simulation wants.
        /// </summary>
        string OnRenderMode();

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game gameMode.
        /// </summary>
        void TickMode();

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game gameMode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        void SendInputBuffer(string returnedLine);

        /// <summary>
        ///     Fired when trail simulation has determined the vehicle and player party has reached the next point of interest in
        ///     the trail.
        /// </summary>
        void OnReachNextLocation(Location nextPoint);

        /// <summary>
        ///     Adds a new game gameMode menu selection that will be available to send as a command for this specific game
        ///     gameMode.
        ///     Description for the enumeration will be taken from it's description attribute, if it does not exist will just be
        ///     enum value.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game gameMode.</param>
        void AddCommand(Action action, Enum command);

        /// <summary>
        ///     Adds a new game gameMode menu selection that will be available to send as a command for this specific game
        ///     gameMode.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game gameMode.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        void AddCommand(Action action, Enum command, string description);

        /// <summary>
        ///     Forces the menu choices to be cleared out, this is used by GameMode like the store to refresh the data shown
        ///     in the
        ///     menu to match purchasing decisions.
        /// </summary>
        void ClearCommands();

        /// <summary>
        ///     Fired by the currently ticking and active game gameMode in the simulation. Implementation is left entirely up to
        ///     concrete handlers for game gameMode. Processes menu items for game gameMode when current state is null, or there
        ///     are no
        ///     menu choices to select from.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, was already checking if null, empty, or whitespace.</param>
        void OnReceiveInputBuffer(string returnedLine);

        /// <summary>
        ///     Fired when this game gameMode is removed from the list of available and ticked GameMode in the simulation.
        /// </summary>
        void OnModeRemoved(GameMode modeType);
    }
}