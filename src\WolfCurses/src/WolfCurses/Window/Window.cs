// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using Form;
    using Menu;

    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    /// <typeparam name="TCommands">Enumeration of all the available commands this window supports.</typeparam>
    /// <typeparam name="TData">Window data class that will be used for this window.</typeparam>
    public abstract class Window<TCommands, TData> :
        Comparer<IWindow>,
        IComparable<Window<TCommands, TData>>,
        IEquatable<Window<TCommands, TData>>,
        IEqualityComparer<Window<TCommands, TData>>,
        IWindow
        where TCommands : struct, IComparable, IFormattable, IConvertible
        where TData : WindowData, new()
    {
        /// <summary>
        ///     Reference to simulation core which houses the window manager and other core modules.
        /// </summary>
        private readonly SimulationApp _simUnit;

        /// <summary>
        ///     Reference for mappings to go from enumeration value to action.
        /// </summary>
        private Dictionary<TCommands, Action> _menuActions;

        /// <summary>
        ///     Keeps track of the total number of menu choices that are currently available to this window.
        /// </summary>
        private int _menuChoiceCount;

        /// <summary>
        ///     Reference to all of the possible commands that this game Windows supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        private List<IMenuChoice<TCommands>> _menuCommands;

        /// <summary>
        ///     Holds the footer text that we will place below menu but before input buffer text.
        /// </summary>
        private string _menuFooter;

        /// <summary>
        ///     Holds the prefix text that can go above the menu text if it exists.
        /// </summary>
        private string _menuHeader;

        /// <summary>
        ///     Reference for mappings to go from string data into a valid loaded input enumeration value.
        /// </summary>
        private Dictionary<string, TCommands> _menuMappings;

        /// <summary>
        ///     Holds the text user interface data that we are going to eventually render out to the user.
        /// </summary>
        private StringBuilder _menuPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        protected Window(SimulationApp simUnit)
        {
            // Copy over reference for simulation core.
            _simUnit = simUnit;

            // Reference for our text user interface data so we can build it up in pieces.
            _menuPrompt = new StringBuilder();

            // Create the user data object casted to correct type from generics while still adhering to common base class.
            UserData = TypeExtensions.New<TData>.Instance();

            // Determines if the menu system should show raw input names in the menu rendering or just number selections by enum value.
            ShowCommandNamesInMenu = SimulationApp.SHOW_COMMANDS;

            // Complain the generics implemented is not of an enum type.
            if (!typeof (TCommands).IsEnum)
            {
                throw new InvalidCastException("TCommands must be an enumerated type!");
            }

            // Create empty list of menu choices.
            _menuCommands = new List<IMenuChoice<TCommands>>();

            // Dictionary of mappings for the choices and their associated values and actions.
            _menuMappings = new Dictionary<string, TCommands>();
            _menuActions = new Dictionary<TCommands, Action>();

            // Menu header and footer is empty strings by default.
            _menuHeader = string.Empty;
            _menuFooter = string.Empty;
        }

        /// <summary>
        ///     Current game Windows state that is being ticked when this Windows is ticked by the underlying simulation.
        /// </summary>
        private IForm Form { get; set; }

        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game Windows might
        ///     need to at the top of menu selections.
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
        ///     Determines if the input names for the particular action should be printed out alongside the number the user can
        ///     press to control that particular enum.
        /// </summary>
        private bool ShowCommandNamesInMenu { get; }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        protected TData UserData { get; }

        /// <summary>
        ///     Because of how generics work in C# we need to have the ability to override a method in implementing classes to get
        ///     back the correct commands for the implementation from abstract class inheritance chain. On the bright side it
        ///     enforces the commands returned to be of the specified enum in generics.
        /// </summary>
        /// <remarks>
        ///     http://stackoverflow.com/a/5042675
        /// </remarks>
        /// <returns>
        ///     Formatting list of enumeration values that can be iterated over as array.
        /// </returns>
        private static TCommands[] Commands
        {
            get
            {
                // Complain the generics implemented is not of an enum type.
                if (!typeof (TCommands).IsEnum)
                {
                    throw new InvalidCastException("T must be an enumerated type!");
                }

                return Enum.GetValues(typeof (TCommands)) as TCommands[];
            }
        }

        /// <summary>The compare to.</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int CompareTo(Window<TCommands, TData> other)
        {
            return Compare(this, other);
        }

        /// <summary>The equals.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool Equals(Window<TCommands, TData> x, Window<TCommands, TData> y)
        {
            return x.Equals(y);
        }

        /// <summary>The get hash code.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int GetHashCode(Window<TCommands, TData> obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>The equals.</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool Equals(Window<TCommands, TData> other)
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

            return GetType().Name.Equals(other.GetType().Name) &&
                   Form.Equals(other.Form);
        }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        WindowData IWindow.UserData
        {
            get { return UserData; }
        }

        /// <summary>
        ///     Determines if the game Windows should not be ticked if it is active but instead removed. The Windows when set to
        ///     being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        public bool ShouldRemoveMode { get; private set; }

        /// <summary>
        ///     Sets the flag for this game Windows to be removed the next time it is ticked by the simulation.
        /// </summary>
        public void RemoveWindowNextTick()
        {
            // Forcefully detaches any state that was active before calling Windows removed.
            ShouldRemoveMode = true;
            Form = null;

            // Allows any data structures that care about themselves to save before the next tick comes.
            OnModeRemoved();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public virtual bool AcceptsInput
        {
            get { return !ShouldRemoveMode; }
        }

        /// <summary>
        ///     Holds the current state which this Windows is in, a Windows will cycle through available states until it is
        ///     finished and
        ///     then detach.
        /// </summary>
        public IForm CurrentForm
        {
            get { return Form; }
        }

        /// <summary>
        ///     Removes the current state from the active game Windows.
        /// </summary>
        public void ClearForm()
        {
            // Don't do anything if the state is already empty.
            if (Form == null)
                return;

            Form = null;
            OnFormChange();
        }

        /// <summary>
        ///     Fired by simulation when it wants to request latest text user interface data for the game Windows, this is used to
        ///     display to user console specific information about what the simulation wants.
        /// </summary>
        /// <returns>
        ///     The windows text user interface<see cref="string" />.
        /// </returns>
        public string OnRenderWindow()
        {
            // Only add menu choices if there are some to actually add, otherwise just return string buffer now.
            _menuPrompt.Clear();
            if (_menuCommands?.Count > 0 && Form == null)
            {
                // Header text for above menu.
                if (!string.IsNullOrEmpty(MenuHeader))
                    _menuPrompt.Append($"{MenuHeader}{Environment.NewLine}{Environment.NewLine}");

                // Figures out which input names and numbers goto what menu choice.
                RefreshCommandMappings();

                // Footer text for below menu.
                if (!string.IsNullOrEmpty(MenuFooter))
                    _menuPrompt.Append($"{MenuFooter}{Environment.NewLine}");
            }
            else
            {
                // Added any descriptive text about the Windows, like stats, health, weather, location, etc.
                var prependMessage = Form?.OnRenderForm();
                if (!string.IsNullOrEmpty(prependMessage))
                    _menuPrompt.Append($"{prependMessage}{Environment.NewLine}");
            }

            // Returns the string buffer we constructed for this game Windows to the simulation so it can be displayed.
            return _menuPrompt.ToString();
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string input
        ///     that should be able to be parsed into a valid input that can be run on the current game Windows.
        /// </summary>
        /// <param name="input">Passed in input from controller, text was trimmed but nothing more.</param>
        public void SendCommand(string input)
        {
            // Only process menu items for game Windows when current state is null, or there are no menu choices to select from.
            if (Form == null &&
                _menuCommands?.Count > 0 &&
                !string.IsNullOrEmpty(input) &&
                !string.IsNullOrWhiteSpace(input))
            {
                // Attempt to convert the returned line into generic enum.
                TCommands parsedCommand;
                Enum.TryParse(input, out parsedCommand);
                if (!(Enum.IsDefined(typeof (TCommands), parsedCommand) |
                      parsedCommand.ToString(CultureInfo.InvariantCulture).Contains(",")))
                    return;

                // Skip if the input does not match any known mapping for menu choices.
                if (!_menuMappings.ContainsKey(input))
                    return;

                // Check if the choice exists in the dictionary of choices valid for this river crossing location.
                if (!_menuActions.ContainsKey(_menuMappings[input]))
                    return;

                // Invoke the anonymous delegate method that was created when this form was attached.
                _menuActions[_menuMappings[input]].Invoke();
            }
            else
            {
                // Skip if current state is null.
                if (Form == null)
                    return;

                // Skip if current state doesn't want our input.
                if (!Form.AllowInput)
                    return;

                // Pass the input buffer to the current state, if it manages to get this far.
                Form.OnInputBufferReturned(input);
            }
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public virtual void OnWindowPostCreate()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Called when the Windows manager in simulation makes this Windows the currently active game Windows. Depending on
        ///     order of modes this might not get called until the Windows is actually ticked by the simulation.
        /// </summary>
        public virtual void OnWindowActivate()
        {
            // Activate the current form, if one exists.
            CurrentForm?.OnFormActivate();
        }

        /// <summary>
        ///     Fired when the simulation adds a game Windows that is not this Windows. Used to execute code in other modes that
        ///     are not
        ///     the active Windows anymore one last time.
        /// </summary>
        public virtual void OnWindowAdded()
        {
            // Nothing to see here, move along...
        }

        /// <summary>The compare.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The <see cref="int" />.</returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override int Compare(IWindow x, IWindow y)
        {
            // ReSharper disable once PossibleNullReferenceException
            var result = string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
            if (result != 0) return result;

            result = x.CurrentForm.CompareTo(y.CurrentForm);
            return result;
        }

        /// <summary>The compare to.</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int CompareTo(IWindow other)
        {
            var result = string.Compare(other.GetType().Name, GetType().Name, StringComparison.Ordinal);
            if (result != 0) return result;

            result = other.CurrentForm.CompareTo(Form);
            return result;
        }

        /// <summary>Creates and adds the specified type of state to currently active game Windows.</summary>
        /// <param name="stateType">The state Type.</param>
        /// <remarks>If Windows does not support given state, an argument exception will be thrown!</remarks>
        public void SetForm(Type stateType)
        {
            // Clear the previous state if something happens.
            if (Form != null)
                ClearForm();

            // States and modes both direct calls to window manager for adding a state.
            Form = _simUnit.WindowManager.CreateStateFromType(this, stateType);

            // Fire method that will allow attaching state to know it is ready for work.
            Form.OnFormPostCreate();

            // Allows underlying parent game Windows to the state understand it changed.
            OnFormChange();
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
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            Form?.OnTick(systemTick, skipDay);
        }

        /// <summary>
        ///     Allows underlying parent game Windows to the state understand it changed.
        /// </summary>
        protected virtual void OnFormChange()
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Adds a new game Windows menu selection that will be available to send as a input for this specific game
        ///     Windows.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated input that will trigger the respective action in the active game Windows.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        private void AddCommand(Action action, TCommands command, string description)
        {
            // Check to make sure the menu choice does not already exist in out collection.
            var menuChoice = new MenuChoice<TCommands>(command, action, description);
            if (_menuCommands.Contains(menuChoice))
                return;

            // Adds the input to the list of possible choices.
            _menuCommands.Add(menuChoice);
        }

        /// <summary>
        ///     Recalculates all of the commands mappings giving them a clear path from a string input
        /// </summary>
        private void RefreshCommandMappings()
        {
            // Clear out any previous mappings.
            _menuMappings.Clear();
            _menuActions.Clear();
            _menuChoiceCount = 0;

            // Loop through every menu input we have.
            foreach (var menuChoice in _menuCommands)
            {
                // Increment the total number of commands we have now.
                _menuChoiceCount++;

                // Add the input to the mapping dictionary, and the delegate for it's action invoker.
                _menuMappings.Add(_menuChoiceCount.ToString(), menuChoice.Command);

                // Adds the reverse mapping for actual enumeration value input to related action.
                _menuActions.Add(menuChoice.Command, menuChoice.Action);

                // Name of input and then description of what it does, the input is all we really care about.
                _menuPrompt.Append(ShowCommandNamesInMenu
                    ? $"  {_menuChoiceCount}. {menuChoice.Command} - {menuChoice.Description}{Environment.NewLine}"
                    : $"  {_menuChoiceCount}. {menuChoice.Description}{Environment.NewLine}");
            }
        }

        /// <summary>
        ///     Adds a new game menu selection with description pulled from attribute on input enumeration. This override is not
        ///     meant for menu selections where you want to manually specify the description of the menu item, this way it will be
        ///     pulled from enum description attribute.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated input that will trigger the respective action in the active game Windows.</param>
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
            // Clears both the commands added by window on create, and the mapping list generated from this class.
            _menuCommands.Clear();
            _menuMappings.Clear();
            _menuActions.Clear();
        }

        /// <summary>Fired when this game Windows is removed from the list of available and ticked modes in the simulation.</summary>
        protected virtual void OnModeRemoved()
        {
            _menuCommands = null;
            _menuMappings = null;
            _menuActions = null;
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        ///     The get hash code.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 23;
            hash = (hash*31) + GetType().Name.GetHashCode();
            return hash;
        }
    }
}