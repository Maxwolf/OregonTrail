using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using TrailSimulation.Game;
using TrailSimulation.Utility;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class ModeProduct<TCommands, TData> :
        Comparer<IModeProduct>,
        IComparable<ModeProduct<TCommands, TData>>,
        IEquatable<ModeProduct<TCommands, TData>>,
        IEqualityComparer<ModeProduct<TCommands, TData>>,
        IModeProduct
        where TCommands : struct, IComparable, IFormattable, IConvertible
        where TData : ModeInfo, new()
    {
        /// <summary>
        ///     Current game mode state that is being ticked when this mode is ticked by the underlying simulation.
        /// </summary>
        private IStateProduct _currentState;

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        private HashSet<IModeChoiceItem<TCommands>> _menuChoices;

        /// <summary>
        ///     Holds the footer text that we will place below menu but before input buffer text.
        /// </summary>
        private string _menuFooter;

        /// <summary>
        ///     Holds the prefix text that can go above the menu text if it exists.
        /// </summary>
        private string _menuHeader;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        protected ModeProduct()
        {
            // Create the user data object casted to correct type from generics while still adhering to common base class.
            UserData = TypeExtensions.New<TData>.Instance();

            // Determines if the menu system should show raw command names in the menu rendering or just number selections by enum value.
            ShowCommandNamesInMenu = SimulationApp.SHOW_COMMANDS;

            // Complain the generics implemented is not of an enum type.
            if (!typeof (TCommands).IsEnum)
            {
                throw new InvalidCastException("TCommands must be an enumerated type!");
            }

            // Create empty list of menu choices.
            _menuChoices = new HashSet<IModeChoiceItem<TCommands>>();

            // Menu header and footer is empty strings by default.
            _menuHeader = string.Empty;
            _menuFooter = string.Empty;
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
        private bool ShowCommandNamesInMenu { get; }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        protected TData UserData { get; }

        public int CompareTo(ModeProduct<TCommands, TData> other)
        {
            return Compare(this, other);
        }

        public bool Equals(ModeProduct<TCommands, TData> x, ModeProduct<TCommands, TData> y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ModeProduct<TCommands, TData> obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(ModeProduct<TCommands, TData> other)
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

            if (Mode.Equals(other.Mode) &&
                _currentState.Equals(other._currentState))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        ModeInfo IModeProduct.UserData
        {
            get { return UserData; }
        }

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
            _currentState = null;

            // Allows any data structures that care about themselves to save before the next tick comes.
            OnModeRemoved(Mode);
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public abstract Mode Mode { get; }

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
        IStateProduct IModeProduct.CurrentState
        {
            get { return _currentState; }
        }

        /// <summary>
        ///     Removes the current state from the active game mode.
        /// </summary>
        public void ClearState()
        {
            // Don't do anything if the state is already empty.
            if (_currentState == null)
                return;

            _currentState = null;
            OnStateChange();
        }

        /// <summary>
        ///     Fired by simulation when it wants to request latest text user interface data for the game mode, this is used to
        ///     display to user console specific information about what the simulation wants.
        /// </summary>
        public string OnRenderMode()
        {
            // Build up string representation of the current state of the game mode.
            var modeTUI = new StringBuilder();

            // Only add menu choices if there are some to actually add, otherwise just return the string buffer now.
            if (_menuChoices?.Count > 0 && _currentState == null)
            {
                // Header text for above menu.
                if (!string.IsNullOrEmpty(MenuHeader))
                    modeTUI.Append($"{MenuHeader}{Environment.NewLine}{Environment.NewLine}");

                // Loop through the menu choices and add each one to the mode text user interface.
                var menuChoices = 1;
                foreach (var menuChoice in _menuChoices)
                {
                    // Name of command and then description of what it does, the command is all we really care about.
                    modeTUI.Append(ShowCommandNamesInMenu
                        ? $"  {menuChoices}. {menuChoice.Command} - {menuChoice.Description}{Environment.NewLine}"
                        : $"  {menuChoices}. {menuChoice.Description}{Environment.NewLine}");

                    // Increment the menu choices number shown to user.
                    menuChoices++;
                }

                // Footer text for below menu.
                if (!string.IsNullOrEmpty(MenuFooter))
                    modeTUI.Append($"{MenuFooter}{Environment.NewLine}");
            }
            else
            {
                // Added any descriptive text about the mode, like stats, health, weather, location, etc.
                var prependMessage = _currentState?.OnRenderState();
                if (!string.IsNullOrEmpty(prependMessage))
                    modeTUI.Append($"{prependMessage}{Environment.NewLine}");
            }

            // Returns the string buffer we constructed for this game mode to the simulation so it can be displayed.
            return modeTUI.ToString();
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        public virtual void OnTick(bool systemTick)
        {
            _currentState?.OnTick(systemTick);
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="command">Passed in command from controller, text was trimmed but nothing more.</param>
        public void SendCommand(string command)
        {
            // Only process menu items for game mode when current state is null, or there are no menu choices to select from.
            if (_currentState == null &&
                _menuChoices?.Count > 0 &&
                !string.IsNullOrEmpty(command) &&
                !string.IsNullOrWhiteSpace(command))
            {
                // Loop through every added menu choice.
                foreach (var menuChoice in _menuChoices)
                {
                    // Attempt to convert the returned line into generic enum.
                    TCommands parsedCommandValue;
                    Enum.TryParse(command, out parsedCommandValue);
                    if (!(Enum.IsDefined(typeof (TCommands), parsedCommandValue) |
                          parsedCommandValue.ToString(CultureInfo.InvariantCulture).Contains(",")))
                        continue;

                    // Check if the received input buffer matches any of them.
                    if (!parsedCommandValue.Equals(menuChoice.Command))
                        continue;

                    // If it matches then invoke the bound action in the simulation.
                    menuChoice.Action.Invoke();
                    return;
                }
            }
            else
            {
                // Skip if current state is null.
                if (_currentState == null)
                    return;

                // Skip if current state doesn't want our input.
                if (!_currentState.AllowInput)
                    return;

                // Pass the input buffer to the current state, if it manages to get this far.
                _currentState.OnInputBufferReturned(command);
            }
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public abstract void OnModePostCreate();

        /// <summary>
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        public abstract void OnModeActivate();

        /// <summary>
        ///     Fired when the simulation adds a game mode that is not this mode. Used to execute code in other modes that are not
        ///     the active mode anymore one last time.
        /// </summary>
        public abstract void OnModeAdded();

        public override int Compare(IModeProduct x, IModeProduct y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = x.Mode.CompareTo(y.Mode);
            if (result != 0) return result;

            result = x.CurrentState.CompareTo(y.CurrentState);
            if (result != 0) return result;

            return result;
        }

        public int CompareTo(IModeProduct other)
        {
            Debug.Assert(other != null, "other != null");

            var result = other.Mode.CompareTo(Mode);
            if (result != 0) return result;

            result = other.CurrentState.CompareTo(_currentState);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Creates and adds the specified type of state to currently active game mode.
        /// </summary>
        /// <remarks>If mode does not support given state, an argument exception will be thrown!</remarks>
        public void SetState(Type stateType)
        {
            // Clear the previous state if something happens.
            if (_currentState != null)
                ClearState();

            // States and modes both direct calls to window manager for adding a state.
            _currentState = GameSimulationApp.Instance.ModeManager.CreateStateFromType(this, stateType);

            // Allows modes to know when they are activated and changed.
            OnStateChange();
        }

        /// <summary>
        ///     Fired when the game mode changes it's internal state. Allows the attached mode to do special behaviors when it
        ///     realizes a state is set or removed and act on it.
        /// </summary>
        protected virtual void OnStateChange()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Because of how generics work in C# we need to have the ability to override a method in implementing classes to get
        ///     back the correct commands for the implementation from abstract class inheritance chain. On the bright side it
        ///     enforces the commands returned to be of the specified enum in generics.
        /// </summary>
        /// <remarks>http://stackoverflow.com/a/5042675</remarks>
        private static TCommands[] GetCommands()
        {
            // Complain the generics implemented is not of an enum type.
            if (!typeof (TCommands).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            return Enum.GetValues(typeof (TCommands)) as TCommands[];
        }

        /// <summary>
        ///     Adds a new game mode menu selection that will be available to send as a command for this specific game mode.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game mode.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        protected void AddCommand(Action action, TCommands command, string description)
        {
            var menuChoice = new ModeChoiceItem<TCommands>(command, action, description);
            if (!_menuChoices.Contains(menuChoice))
            {
                _menuChoices.Add(menuChoice);
            }
        }

        /// <summary>
        ///     Adds a new game menu selection with description pulled from attribute on command enumeration. This override is not
        ///     meant for menu selections where you want to manually specify the description of the menu item, this way it will be
        ///     pulled from enum description attribute.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game mode.</param>
        protected void AddCommand(Action action, TCommands command)
        {
            AddCommand(action, command, command.ToDescriptionAttribute());
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
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        protected virtual void OnModeRemoved(Mode mode)
        {
            _menuChoices = null;
        }

        public override string ToString()
        {
            return Mode.ToString();
        }

        public override int GetHashCode()
        {
            var hash = 23;
            hash = (hash*31) + Mode.GetHashCode();
            return hash;
        }
    }
}