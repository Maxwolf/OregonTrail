using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Base simulation class that deals with ticks, time, named pipes, and game modes.
    /// </summary>
    public abstract class SimulationApp : TickSim, ISimulation
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
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        private List<IMode> _modes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            // References all of the active game modes that need to be ticked.
            _modes = new List<IMode>();

            // Start with a clear input and screen buffer.
            InputBuffer = string.Empty;
            ScreenBuffer = string.Empty;
        }

        /// <summary>
        ///     Removes the active game mode from the list of available modes. The only requirements are that it exists, and is
        ///     currently active.
        /// </summary>
        public void RemoveActiveMode()
        {
            // Ensure the mode exists as active mode.
            if (ActiveMode == null)
                throw new InvalidOperationException("Attempted to remove active mode when it is null!");

            // Ensure modes list contains active mode.
            if (!_modes.Contains(ActiveMode))
                throw new InvalidOperationException("Active mode is not in list of current modes!");

            // Remove the mode from list.
            _modes.Remove(ActiveMode);

            // Check if there are any modes after removal.
            if (ActiveMode != null)
                ModeChangedEvent?.Invoke(ActiveMode.ModeType);
        }

        /// <summary>
        ///     Holds the last known representation of the game simulation and current mode text user interface, only pushes update
        ///     when a change occurs.
        /// </summary>
        public string ScreenBuffer { get; internal set; }

        /// <summary>
        ///     Input buffer that we will use to hold characters until need to send them to simulation.
        /// </summary>
        public string InputBuffer { get; internal set; }

        /// <summary>
        ///     Removes the last character from input buffer if greater than zero.
        /// </summary>
        public void RemoteLastCharOfInputBuffer()
        {
            if (InputBuffer.Length > 0)
                InputBuffer = InputBuffer.Remove(InputBuffer.Length - 1);
        }

        /// <summary>
        ///     Clears the input buffer and submits whatever was in there to the simulation for processing. Implementation is left
        ///     up the game simulation itself entirely.
        /// </summary>
        public void SendInputBuffer()
        {
            // Trim the result of the input so no extra whitespace at front or end exists.
            var lineBufferTrimmed = InputBuffer.Trim();

            // Send trimmed line buffer to game simulation.
            OnInputBufferReturned(lineBufferTrimmed);
            InputBuffer = string.Empty;
        }

        /// <summary>
        ///     Populates an internal input buffer for the simulation that is used to eventually return a possible command string
        ///     to active game mode.
        /// </summary>
        public void SendKeyCharToInputBuffer(char keyChar)
        {
            // Filter to prevent non-characters like delete, insert, scroll lock, etc.
            if (!char.IsLetter(keyChar) && !char.IsNumber(keyChar))
                return;

            // Convert character to string representation if itself.
            var addedKeyString = char.ToString(keyChar);
            OnKeyCharAddedToInputBuffer(addedKeyString);
        }

        /// <summary>
        ///     Use screen buffer to only write to console when something actually changes to stop flickering from constant
        ///     ticking.
        /// </summary>
        public void TickTUI()
        {
            // Get the current text user interface data from inheriting class.
            var tuiContent = OnTickTUI();
            if (ScreenBuffer.Equals(tuiContent, StringComparison.InvariantCultureIgnoreCase))
                return;

            // Update the screen buffer with altered data.
            ScreenBuffer = tuiContent;
            ScreenBufferDirtyEvent?.Invoke(ScreenBuffer);
        }

        /// <summary>
        ///     Determines if this simulation is currently accepting input at all, the conditions for this require some game mode
        ///     to be attached and or active move to not be null.
        /// </summary>
        public bool AcceptingInput
        {
            get { return Modes?.Count > 0 || ActiveMode != null; }
        }

        /// <summary>
        ///     References the current active game mode, or the last attached game mode in the simulation.
        /// </summary>
        public IMode ActiveMode
        {
            get
            {
                if (_modes.Count <= 0)
                    return null;

                var lastMode = _modes[_modes.Count - 1];
                return lastMode;
            }
        }

        /// <summary>
        ///     Returns the name of the currently active game mode, otherwise will return "None".
        /// </summary>
        public string ActiveModeName
        {
            get
            {
                if (_modes.Count <= 0)
                    return "None";

                var lastMode = _modes[_modes.Count - 1];
                return lastMode.ModeType.ToString();
            }
        }

        /// <summary>
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        public ReadOnlyCollection<IMode> Modes
        {
            get { return _modes.AsReadOnly(); }
        }

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event ScreenBufferDirty ScreenBufferDirtyEvent;
        public event InputBufferUpdated InputBufferUpdatedEvent;
        public event ModeChanged ModeChangedEvent;

        /// <summary>
        ///     Creates and adds the specified game mode to the simulation if it does not already exist in the list of modes.
        /// </summary>
        /// <param name="modeType">Enumeration value of the mode which should be created.</param>
        public void AddMode(ModeType modeType)
        {
            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(modeType);
            if (_modes.Contains(changeMode))
                return;

            _modes.Add(changeMode);
            ModeChangedEvent?.Invoke(changeMode.ModeType);
        }

        /// <summary>
        ///     Fired when the simulation receives an individual character from then input system. Depending on what it is we will
        ///     do something, or not!
        /// </summary>
        /// <param name="addedKeyString">Character converted into a string representation of itself.</param>
        protected virtual void OnKeyCharAddedToInputBuffer(string addedKeyString)
        {
            // Add the character to the end of the input buffer.
            InputBuffer += addedKeyString;

            // Fire event for all subscribers to get total buffer and added character string.
            InputBufferUpdatedEvent?.Invoke(InputBuffer, addedKeyString);
        }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        protected virtual string OnTickTUI()
        {
            // If TUI for active game mode is not null or empty then use it.
            var activeModeTUI = ActiveMode?.GetTUI();
            if (!string.IsNullOrEmpty(activeModeTUI))
                return activeModeTUI;

            // Otherwise, display default message if null for mode.
            return ActiveMode == null ? GAMEMODE_EMPTY_TUI : GAMEMODE_DEFAULT_TUI;
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public abstract void OnInputBufferReturned(string returnedLine);

        /// <summary>
        ///     Attaches the traveling mode and removes the new game mode if it exists, this begins the simulation down the trail
        ///     path and all the points of interest on it.
        /// </summary>
        public void StartGame()
        {
            NewgameEvent?.Invoke();
        }

        protected abstract IMode OnModeChanging(ModeType modeType);

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            InputBuffer = string.Empty;
            ScreenBuffer = string.Empty;
            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        /// <summary>
        ///     Fired when the system timers timer elapses it's interval and fires event that this encompasses.
        /// </summary>
        protected override void OnTimerTick()
        {
            base.OnTimerTick();

            TickModes();
        }

        /// <summary>
        ///     Ticked by the underlying operating system through singleton instance of game simulation.
        /// </summary>
        protected override void OnSystemTick()
        {
            base.OnSystemTick();

            TickTUI();
        }

        /// <summary>
        ///     Process top-most game mode logic.
        /// </summary>
        private void TickModes()
        {
            ActiveMode?.TickMode();
        }
    }
}