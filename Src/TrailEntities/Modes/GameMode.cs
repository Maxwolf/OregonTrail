using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public abstract class GameMode<T> : IMode where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        private HashSet<IModeChoiceItem<T>> _menuChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode()
        {
            // Complain the generics implemented is not of an enum type.
            if (!typeof (T).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            // Create empty list of menu choices.
            _menuChoices = new HashSet<IModeChoiceItem<T>>();
        }

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        public ReadOnlyCollection<IModeChoiceItem<T>> MenuChoices
        {
            get { return _menuChoices.ToList().AsReadOnly(); }
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public abstract ModeType ModeType { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public abstract bool AcceptsInput { get; }

        /// <summary>
        ///     Holds the current state which this mode is in, a mode will cycle through available states until it is finished and
        ///     then detach.
        /// </summary>
        public IModeState CurrentState { get; set; }

        /// <summary>
        ///     Calls the abstract methods with generics using this override.
        /// </summary>
        /// <returns>Formatted list of command enumeration values in an array (since I cannot return System.Enum directly).</returns>
        object[] IMode.GetCommands()
        {
            return new object[] {GetCommands()};
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
            var prependMessage = CurrentState?.GetStateTUI();
            if (!string.IsNullOrEmpty(prependMessage))
                modeTUI.Append(prependMessage + "\n");

            // Only add menu choices if there are some to actually add, otherwise just return the string buffer now.
            if (_menuChoices.Count > 0 && CurrentState == null)
            {
                // Loop through the menu choices and add each one to the mode text user interface.
                foreach (var menuChoice in _menuChoices)
                {
                    // Name of command and then description of what it does, the command is all we really care about.
                    modeTUI.AppendFormat("  {0} - {1}\n", menuChoice.Command, menuChoice.Description);
                }
            }

            // Returns the string buffer we constructed for this game mode to the simulation so it can be displayed.
            return modeTUI.ToString();
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public virtual void TickMode()
        {
            // Tick the logic in the current state if it not null.
            CurrentState?.TickState();
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public void SendInputBuffer(string returnedLine)
        {
            OnReceiveInputBuffer(returnedLine);
        }

        /// <summary>
        /// Fired when the active game mode has been changed, this allows any underlying mode to know about a change in simulation.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        public virtual void OnModeChanged(ModeType modeType)
        {
            // Pass info along if current state exists.
            CurrentState?.OnParentModeChanged(modeType);
        }

        /// <summary>
        ///     Because of how generics work in C# we need to have the ability to override a method in implementing classes to get
        ///     back the correct commands for the implementation from abstract class inheritance chain. On the bright side it
        ///     enforces the commands returned to be of the specified enum in generics.
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/5042675</remarks>
        private static T[] GetCommands()
        {
            // Complain the generics implemented is not of an enum type.
            if (!typeof (T).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            return Enum.GetValues(typeof (T)) as T[];
        }

        /// <summary>
        ///     Adds a new game mode menu selection that will be available to send as a command for this specific game mode.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game mode.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        protected void AddCommand(Action action, T command, string description)
        {
            var menuChoice = new ModeChoiceItem<T>(command, action, description);
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
        private void OnReceiveInputBuffer(string returnedLine)
        {
            // Only process menu items for game mode when current state is null, or there are no menu choices to select from.
            if (CurrentState == null && _menuChoices.Count > 0)
            {
                // Loop through every added menu choice.
                foreach (var menuChoice in _menuChoices)
                {
                    try
                    {
                        // Attempt to convert the returned line into generic enum.
                        var parsedCommandValue = (T) Enum.Parse(typeof (T), returnedLine, true);
                        if (Enum.IsDefined(typeof (T), parsedCommandValue) |
                            parsedCommandValue.ToString(CultureInfo.InvariantCulture).Contains(","))
                        {
                            // Check if the received input buffer matches any of them.
                            if (!parsedCommandValue.Equals(menuChoice.Command))
                                continue;

                            // If it matches then invoke the bound action in the simulation.
                            menuChoice.Action.Invoke();
                            return;
                        }
                    }
                    catch (ArgumentException)
                    {
                        // Pass input buffer to current state if it doesn't match any known command.
                        break;
                    }
                }
            }
            else
            {
                // Pass the input buffer to the current state, if it manages to get this far.
                CurrentState?.OnInputBufferReturned(returnedLine);
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
            return ModeType.ToString();
        }
    }
}