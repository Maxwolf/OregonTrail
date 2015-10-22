using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class GameMode : IMode
    {
        /// <summary>
        ///     Default string used when game mode has nothing better to say.
        /// </summary>
        public const string GAMEMODE_DEFAULT_TUI = "[DEFAULT GAME MODE TEXT USER INTERFACE]";

        /// <summary>
        ///     Default string used when there are no game modes at all.
        /// </summary>
        public const string GAMEMODE_EMPTY_TUI = "[NO GAME MODE ATTACHED]";

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        private HashSet<IModeChoice> _menuChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode()
        {
            _menuChoices = new HashSet<IModeChoice>();
        }

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        public ReadOnlyCollection<IModeChoice> MenuChoices
        {
            get { return new ReadOnlyCollection<IModeChoice>(_menuChoices.ToArray()); }
        }

        /// <summary>
        ///     Fired by simulation when it wants to request latest text user interface data for the game mode, this is used to
        ///     display to user console specific information about what the simulation wants.
        /// </summary>
        public string GetTUI()
        {
            // Build up string representation of the current state of the game mode.
            var modeTUI = new StringBuilder();

            // Added any descriptive text about the mode, like stats, health, weather, location, etc.
            var prependMessage = OnGetModeTUI();
            if (prependMessage != string.Empty)
                modeTUI.Append(prependMessage + "\n");

            // Only add menu choices if there are some to actually add, otherwise just return the string buffer now.
            if (_menuChoices.Count <= 0)
                return modeTUI.ToString();

            // Loop through the menu choices and add each one to the mode text user interface.
            foreach (var menuChoice in _menuChoices)
            {
                // Name of command and then description of what it does, the command is all we really care about.
                modeTUI.AppendFormat("  {0} - {1}\n", menuChoice.CommandName.ToUpper(), menuChoice.Description);
            }

            // Returns the string buffer we constructed for this game mode to the simulation so it can be displayed.
            return modeTUI.ToString();
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public abstract ModeType Mode { get; }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public abstract void TickMode();

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public void ProcessCommand(string returnedLine)
        {
            if (!string.IsNullOrEmpty(returnedLine) ||
                !string.IsNullOrWhiteSpace(returnedLine))
            {
                OnReceiveInputBuffer(returnedLine);
            }
        }

        /// <summary>
        ///     Called by the active game mode when the text user interface is called. This will create a string builder with all
        ///     the data and commands that represent the concrete handler for this game mode.
        /// </summary>
        protected abstract string OnGetModeTUI();

        /// <summary>
        ///     Adds a new game mode menu selection that will be available to send as a command for this specific game mode.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="commandText">Associated command text that will trigger the respective action in the active game mode.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        public void AddCommand(Action action, string commandText, string description)
        {
            var menuChoice = new GameModeChoice(action, description, commandText);
            if (!_menuChoices.Contains(menuChoice))
            {
                _menuChoices.Add(menuChoice);
            }
        }

        /// <summary>
        ///     Fired by the currently ticking and active game mode in the simulation. Implementation is left entirely up to
        ///     concrete handlers for game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, was already checking if null, empty, or whitespace.</param>
        protected void OnReceiveInputBuffer(string returnedLine)
        {
            // Loop through every added menu choice.
            foreach (var menuChoice in _menuChoices)
            {
                // Check if the received input buffer matches any of them.
                if (!returnedLine.Equals(menuChoice.CommandName, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                // If it matches then invoke the bound action in the simulation.
                menuChoice.Action.Invoke();
                break;
            }
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public virtual void OnModeRemoved()
        {
            _menuChoices = null;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Mode.ToString();
        }
    }
}