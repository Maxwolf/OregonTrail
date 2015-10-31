namespace TrailCommon
{
    /// <summary>
    ///     Underlying game mode interface, used by base simulation to keep track of what data should currently have control
    ///     over the simulation details. Only top most game mode will ever be ticked.
    /// </summary>
    public interface IMode
    {
        /// <summary>
        ///     Determines if the game mode should not be ticked if it is active but instead removed. The mode when set to being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        bool ShouldRemoveMode { get; }

        /// <summary>
        ///     Defines the type of game mode this is and what it's purpose will be intended for.
        /// </summary>
        ModeType ModeType { get; }

        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game mode might need
        ///     to at the top of menu selections.
        /// </summary>
        string MenuHeader { get; }

        /// <summary>
        ///     Similar to the header this will define some text that should go below the menu selection but before the user input
        ///     field.
        /// </summary>
        string MenuFooter { get; }

        /// <summary>
        ///     Determines if the command names for the particular action should be printed out alongside the number the user can
        ///     press to control that particular enum.
        /// </summary>
        bool ShowCommandNamesInMenu { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Holds the current state which this mode is in, a mode will cycle through available states until it is finished and
        ///     then detach.
        /// </summary>
        IModeState CurrentState { get; set; }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        IPoint CurrentPoint { get; }

        /// <summary>
        ///     Sets the flag for this game mode to be removed the next time it is ticked by the simulation.
        /// </summary>
        void RemoveModeNextTick();

        /// <summary>
        ///     Because of how generics work in C# we need to have the ability to override a method in implementing classes to get
        ///     back the correct commands for the implementation from abstract class inheritance chain.
        /// </summary>
        object[] GetCommands();

        /// <summary>
        ///     Grabs the text user interface string that will be used for debugging on console application.
        /// </summary>
        string GetTUI();

        /// <summary>
        ///     Ticks the internal logic of the game mode so that it may perform linear operations.
        /// </summary>
        void TickMode();

        /// <summary>
        ///     Intended to be used by base simulation to pass along the input buffer after the user has typed several characters
        ///     into the input buffer. This is used when allowing the user to input custom strings like names for their party
        ///     members.
        /// </summary>
        void SendInputBuffer(string returnedLine);

        /// <summary>
        ///     Fired when the active game mode has been changed, this allows any underlying mode to know about a change in
        ///     simulation.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        void OnModeChanged(ModeType modeType);
    }
}