using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class GameMode<T> : Comparer<IMode>, IComparable<GameMode<T>>, IEquatable<GameMode<T>>,
        IEqualityComparer<GameMode<T>>, IMode where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Keeps track of the current game mode state, if null then there is no state attached and menu is shown.
        /// </summary>
        private IModeState _currentState;

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        private HashSet<IModeChoiceItem<T>> _menuChoices;

        /// <summary>
        ///     Holds the footer text that we will place below menu but before input buffer text.
        /// </summary>
        private string _menuFooter;

        /// <summary>
        ///     Holds the prefix text that can go above the menu text if it exists.
        /// </summary>
        private string _menuHeader;

        /// <summary>
        ///     Determines if the command names for the particular action should be printed out alongside the number the user can
        ///     press to control that particular enum.
        /// </summary>
        private bool _showCommandNamesInMenu;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode(bool showCommandNamesInMenu)
        {
            // Determines if the menu system should show raw command names in the menu rendering or just number selections by enum value.
            _showCommandNamesInMenu = showCommandNamesInMenu;

            // Complain the generics implemented is not of an enum type.
            if (!typeof (T).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            // Create empty list of menu choices.
            _menuChoices = new HashSet<IModeChoiceItem<T>>();

            // Menu header and footer is empty strings by default.
            _menuHeader = string.Empty;
            _menuFooter = string.Empty;

            // Hook event to know when we have reached a location point of interest.
            GameSimulationApp.Instance.TrailSim.OnReachPointOfInterest += OnReachPointOfInterest;

            // Cast the current point of interest into a settlement object since that is where stores are.
            CurrentPoint = GameSimulationApp.Instance.TrailSim.GetCurrentPointOfInterest();
            if (CurrentPoint == null)
                throw new InvalidCastException("Unable to get current point of interest from trail simulation!");
        }

        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game mode might need
        ///     to at the top of menu selections.
        /// </summary>
        protected virtual string MenuHeader
        {
            get { return _menuHeader; }
            set { _menuHeader = value; }
        }

        /// <summary>
        ///     Similar to the header this will define some text that should go below the menu selection but before the user input
        ///     field.
        /// </summary>
        protected virtual string MenuFooter
        {
            get { return _menuFooter; }
            set { _menuFooter = value; }
        }

        /// <summary>
        ///     Determines if the command names for the particular action should be printed out alongside the number the user can
        ///     press to control that particular enum.
        /// </summary>
        public virtual bool ShowCommandNamesInMenu
        {
            get { return _showCommandNamesInMenu; }
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(GameMode<T> other)
        {
            return Compare(this, other);
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(GameMode<T> x, GameMode<T> y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(GameMode<T> obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(GameMode<T> other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (ModeType.Equals(other.ModeType) &&
                _currentState.Equals(other._currentState))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        public IPoint CurrentPoint { get; }

        /// <summary>
        ///     Determines if the game mode should not be ticked if it is active but instead removed. The mode when set to being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        public bool ShouldRemoveMode { get; private set; }

        /// <summary>
        ///     Sets the flag for this game mode to be removed the next time it is ticked by the simulation.
        /// </summary>
        public void RemoveModeNextTick()
        {
            // Forcefully detaches any state that was active before calling mode removed.
            ShouldRemoveMode = true;
            CurrentState = null;

            // Allows any data structures that care about themselves to save before the next tick comes.
            OnModeRemoved(ModeType);
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
        public virtual bool AcceptsInput
        {
            get { return !ShouldRemoveMode; }
        }

        /// <summary>
        ///     Holds the current state which this mode is in, a mode will cycle through available states until it is finished and
        ///     then detach.
        /// </summary>
        public IModeState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                OnStateChanged();
            }
        }

        /// <summary>
        ///     Fired by simulation when it wants to request latest text user interface data for the game mode, this is used to
        ///     display to user console specific information about what the simulation wants.
        /// </summary>
        public string GetTUI()
        {
            // Build up string representation of the current state of the game mode.
            var modeTUI = new StringBuilder();

            // Only add menu choices if there are some to actually add, otherwise just return the string buffer now.
            if (_menuChoices?.Count > 0 && CurrentState == null)
            {
                // Header text for above menu.
                if (!string.IsNullOrEmpty(MenuHeader))
                    modeTUI.Append(MenuHeader + "\n\n");

                // Loop through the menu choices and add each one to the mode text user interface.
                var menuChoices = 1;
                foreach (var menuChoice in _menuChoices)
                {
                    // Name of command and then description of what it does, the command is all we really care about.
                    modeTUI.Append(_showCommandNamesInMenu
                        ? $"  {menuChoices}. {menuChoice.Command} - {menuChoice.Description}\n"
                        : $"  {menuChoices}. {menuChoice.Description}\n");

                    // Increment the menu choices number shown to user.
                    menuChoices++;
                }

                // Footer text for below menu.
                if (!string.IsNullOrEmpty(MenuFooter))
                    modeTUI.Append(MenuFooter + "\n");
            }
            else
            {
                // Added any descriptive text about the mode, like stats, health, weather, location, etc.
                var prependMessage = CurrentState?.GetStateTUI();
                if (!string.IsNullOrEmpty(prependMessage))
                    modeTUI.Append(prependMessage + "\n");
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
        ///     Fired when the active game mode has been changed, this allows any underlying mode to know about a change in
        ///     simulation.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        public virtual void OnModeChanged(ModeType modeType)
        {
            // Pass info along if current state exists.
            CurrentState?.OnParentModeChanged();
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public override int Compare(IMode x, IMode y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = x.ModeType.CompareTo(y.ModeType);
            if (result != 0) return result;

            result = x.CurrentState.CompareTo(y.CurrentState);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IMode other)
        {
            Debug.Assert(other != null, "other != null");

            var result = other.ModeType.CompareTo(ModeType);
            if (result != 0) return result;

            result = other.CurrentState.CompareTo(CurrentState);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Fired when trail simulation has determined the vehicle and player party has reached the next point of interest in
        ///     the trail.
        /// </summary>
        protected virtual void OnReachPointOfInterest(PointOfInterest nextPoint)
        {
            Debug.Assert(nextPoint != null, "nextPoint != null");
        }

        /// <summary>
        ///     Fired when the current game modes state is altered, it could be removed and null or a new one added up to
        ///     implementation to check.
        /// </summary>
        protected virtual void OnStateChanged()
        {
            // Nothing to see here, move along...
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
        ///     Forces the menu choices to be cleared out, this is used by modes like the store to refresh the data shown in the
        ///     menu to match purchasing decisions.
        /// </summary>
        protected void ClearCommands()
        {
            _menuChoices.Clear();
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
        /// <param name="modeType">The mode that is about to be removed.</param>
        protected virtual void OnModeRemoved(ModeType modeType)
        {
            GameSimulationApp.Instance.TrailSim.OnReachPointOfInterest -= OnReachPointOfInterest;
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

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 23;
            hash = (hash*31) + CurrentState.GetHashCode();
            hash = (hash*31) + ModeType.GetHashCode();
            return hash;
        }
    }
}