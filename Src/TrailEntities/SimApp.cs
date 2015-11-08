using System;
using System.Collections.Generic;
using System.Linq;

namespace TrailEntities
{
    /// <summary>
    ///     Base simulation class that deals with ticks, time, named pipes, and game modes.
    /// </summary>
    public abstract class SimApp : TickSim
    {
        public delegate void EndGame();

        public delegate void InputBufferUpdated(string inputBuffer, string addedKeycharString);

        public delegate void ModeChanged(ModeType modeType);

        public delegate void NewGame();

        public delegate void ScreenBufferDirty(string tuiContent);

        /// <summary>
        ///     Default string used when game mode has nothing better to say.
        /// </summary>
        private const string GAMEMODE_DEFAULT_TUI = "[DEFAULT GAME MODE TEXT USER INTERFACE]";

        /// <summary>
        ///     Default string used when there are no game modes at all.
        /// </summary>
        private const string GAMEMODE_EMPTY_TUI = "[NO GAME MODE ATTACHED]";

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimApp" /> class.
        /// </summary>
        protected SimApp()
        {
            // References all of the active game modes that need to be ticked.
            Modes = new Dictionary<ModeType, IMode>();

            // Start with a clear input and screen buffer.
            InputBuffer = string.Empty;
            ScreenBuffer = string.Empty;
        }

        /// <summary>
        ///     Holds the last known representation of the game simulation and current mode text user interface, only pushes update
        ///     when a change occurs.
        /// </summary>
        private string ScreenBuffer { get; set; }

        /// <summary>
        ///     Input buffer that we will use to hold characters until need to send them to simulation.
        /// </summary>
        protected string InputBuffer { get; private set; }

        /// <summary>
        ///     Determines if this simulation is currently accepting input at all, the conditions for this require some game mode
        ///     to be attached and or active move to not be null.
        /// </summary>
        protected bool AcceptingInput
        {
            get
            {
                return (ActiveMode != null && ActiveMode.CurrentState == null && ActiveMode.AcceptsInput) ||
                       (ActiveMode?.CurrentState != null && ActiveMode.AcceptsInput &&
                        ActiveMode.CurrentState.AcceptsInput);
            }
        }

        /// <summary>
        ///     References the current active game mode, or the last attached game mode in the simulation.
        /// </summary>
        protected IMode ActiveMode
        {
            get { return Modes.LastOrDefault().Value; }
        }

        /// <summary>
        ///     Returns the total number of active game modes that are currently loaded into the simulation.
        /// </summary>
        public int ModeCount
        {
            get { return Modes.Count; }
        }

        /// <summary>
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        protected Dictionary<ModeType, IMode> Modes { get; }

        /// <summary>
        ///     Removes any and all inactive game modes that need to be removed from the simulation.
        /// </summary>
        private void RemoveDirtyModes()
        {
            // Ensure the mode exists as active mode.
            if (ActiveMode == null)
                throw new InvalidOperationException("Attempted to remove active mode when it is null!");

            // Create copy of all modes so we can destroy while iterating.
            var copyModes = new Dictionary<ModeType, IMode>(Modes);
            foreach (var mode in copyModes)
            {
                // Skip if the mode doesn't want to be removed.
                if (!mode.Value.ShouldRemoveMode)
                    continue;

                // Remove the mode from list if it is flagged for removal.
                Modes.Remove(mode.Key);

                // Fire virtual method which will allow game simulation above and attempt to pass this data along to internal game mode and game mode states.
                if (ActiveMode != null)
                    OnModeChanged(ActiveMode.ModeType);
            }
            copyModes.Clear();
        }

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

            // Destroy the input buffer if we are not accepting commands but return is pressed anyway.
            if (!AcceptingInput)
                InputBuffer = string.Empty;

            // Send trimmed line buffer to game simulation, if not accepting input we just pass along empty string.
            OnInputBufferReturned(lineBufferTrimmed);

            // Always forcefully clear the input buffer after returning it, this makes it ready for more input.
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
        private void TickTUI()
        {
            // Get the current text user interface data from inheriting class.
            var tuiContent = RenderMode();
            if (ScreenBuffer.Equals(tuiContent, StringComparison.InvariantCultureIgnoreCase))
                return;

            // Update the screen buffer with altered data.
            ScreenBuffer = tuiContent;
            ScreenBufferDirtyEvent?.Invoke(ScreenBuffer);
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
            // Check if any other modes match the one we are adding.
            if (Modes.ContainsKey(modeType))
                return;

            // Add the game mode to the simulation now that we know it does not exist in the stack yet.
            Modes.Add(modeType, OnModeChange(modeType));
            ModeChangedEvent?.Invoke(Modes[modeType].ModeType);
        }

        /// <summary>
        ///     Fired when the active game mode has been changed, this allows any underlying mode to know about a change in
        ///     simulation.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        private void OnModeChanged(ModeType modeType)
        {
            // Fire event that lets subscribers know we changed something.
            ModeChangedEvent?.Invoke(modeType);

            // Pass the information along to active game mode.
            ActiveMode?.OnModeChanged(modeType);
        }

        /// <summary>
        ///     Fired when the simulation receives an individual character from then input system. Depending on what it is we will
        ///     do something, or not!
        /// </summary>
        /// <param name="addedKeyString">Character converted into a string representation of itself.</param>
        private void OnKeyCharAddedToInputBuffer(string addedKeyString)
        {
            // Disable passing along input buffer if the simulation is not currently accepting input from the user.
            if (!AcceptingInput)
                return;

            // Add the character to the end of the input buffer.
            InputBuffer += addedKeyString;

            // Fire event for all subscribers to get total buffer and added character string.
            InputBufferUpdatedEvent?.Invoke(InputBuffer, addedKeyString);
        }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        protected virtual string RenderMode()
        {
            // If TUI for active game mode is not null or empty then use it.
            var activeModeTUI = ActiveMode?.OnRenderMode();
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
        protected abstract void OnInputBufferReturned(string returnedLine);

        /// <summary>
        ///     Attaches the traveling mode and removes the new game mode if it exists, this begins the simulation down the trail
        ///     path and all the points of interest on it.
        /// </summary>
        /// <param name="startingInfo">User data object that was passed around the new game mode and populated by user selections.</param>
        public virtual void SetData(MainMenuInfo startingInfo)
        {
            NewgameEvent?.Invoke();
        }

        protected abstract IMode OnModeChange(ModeType modeType);

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            InputBuffer = string.Empty;
            ScreenBuffer = string.Empty;
            Modes.Clear();
            EndgameEvent?.Invoke();
        }

        /// <summary>
        ///     Fired when the system timers timer elapses it's interval and fires event that this encompasses.
        /// </summary>
        protected override void OnTick()
        {
            base.OnTick();

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
            // If the active mode is not null and flag is set to remove then do that!
            if (ActiveMode != null && ActiveMode.ShouldRemoveMode)
                RemoveDirtyModes();

            // Otherwise just tick the game mode logic.
            ActiveMode?.TickMode();
        }
    }
}