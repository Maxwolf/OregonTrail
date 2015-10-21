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
                ModeChangedEvent?.Invoke(ActiveMode.Mode);
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

        public void ProcessInputBuffer()
        {
            // Only process key press if something is there.
            var lineBufferTrimmed = InputBuffer.Trim();
            if (string.IsNullOrEmpty(lineBufferTrimmed))
                return;

            // Send trimmed line buffer to game simulation.
            SendCommand(lineBufferTrimmed);
            InputBuffer = string.Empty;
        }

        /// <summary>
        ///     Populates an internal input buffer for the simulation that is used to eventually return a possible command string
        ///     to active game mode.
        /// </summary>
        /// <param name="keyChar"></param>
        public void SendKeyCharString(char keyChar)
        {
            // Filter to prevent non-characters like delete, insert, scroll lock, etc.
            if (char.IsLetter(keyChar) || char.IsNumber(keyChar))
            {
                InputBuffer += char.ToString(keyChar);
            }
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
                return lastMode.Mode.ToString();
            }
        }

        /// <summary>
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        public ReadOnlyCollection<IMode> Modes
        {
            get { return new ReadOnlyCollection<IMode>(_modes); }
        }

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event ScreenBufferDirty ScreenBufferDirtyEvent;
        public event ModeChanged ModeChangedEvent;

        /// <summary>
        ///     Creates and adds the specified game mode to the simulation if it does not already exist in the list of modes.
        /// </summary>
        /// <param name="mode">Enumeration value of the mode which should be created.</param>
        public void AddMode(ModeType mode)
        {
            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(mode);
            if (_modes.Contains(changeMode))
                return;

            _modes.Add(changeMode);
            ModeChangedEvent?.Invoke(changeMode.Mode);
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
            return ActiveMode == null ? GameMode.GAMEMODE_EMPTY_TUI : GameMode.GAMEMODE_DEFAULT_TUI;
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public virtual void SendCommand(string returnedLine)
        {
            if (!string.IsNullOrEmpty(returnedLine) ||
                !string.IsNullOrWhiteSpace(returnedLine))
            {
                OnReceiveCommand(returnedLine);
            }
        }

        /// <summary>
        ///     Fired by the currently ticking and active game mode in the simulation. Implementation is left entirely up to
        ///     concrete handlers for game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, was already checking if null, empty, or whitespace.</param>
        protected abstract void OnReceiveCommand(string returnedLine);

        public void StartGame()
        {
            NewgameEvent?.Invoke();
        }

        protected abstract GameMode OnModeChanging(ModeType mode);

        public override void OnDestroy()
        {
            base.OnDestroy();

            InputBuffer = string.Empty;
            ScreenBuffer = string.Empty;
            _modes.Clear();
            EndgameEvent?.Invoke();
        }

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