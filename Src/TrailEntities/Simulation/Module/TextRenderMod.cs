using System;
using System.Text;
using TrailEntities.Game;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Provides base functionality for rendering out the simulation state via text user interface (TUI). This class has no
    ///     idea about how other modules work and only serves to query them for string data which will be compiled into a
    ///     console only view of the simulation which is intended to be the lowest level of visualization but theoretically
    ///     anything could be a renderer for the simulation.
    /// </summary>
    public sealed class TextRenderMod : SimulationMod
    {
        /// <summary>
        ///     Fired when the screen back buffer has changed from what is currently being shown, this forces a redraw.
        /// </summary>
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
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.TextRender" /> class.
        /// </summary>
        public TextRenderMod()
        {
            ScreenBuffer = string.Empty;
        }

        /// <summary>
        ///     Holds the last known representation of the game simulation and current mode text user interface, only pushes update
        ///     when a change occurs.
        /// </summary>
        private string ScreenBuffer { get; set; }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        private string OnRender()
        {
            // Spinning ticker that shows activity, lets us know if application hangs or freezes.
            var tui = new StringBuilder();
            var windowMan = GameSimulationApp.Instance.WindowManager;
            tui.Append($"[ {GameSimulationApp.Instance.Ticker.TickPhase} ] - ");

            // Keeps track of active mode name and active mode current state name for debugging purposes.
            tui.Append(windowMan.ActiveMode?.CurrentState != null
                ? $"Mode({windowMan.Modes.Count}): {windowMan.ActiveMode}({windowMan.ActiveMode.CurrentState}) - "
                : $"Mode({windowMan.Modes.Count}): {windowMan.ActiveMode}(NO STATE) - ");

            // Total number of turns that have passed in the simulation.
            tui.Append($"Turns: {GameSimulationApp.Instance.TotalTurns.ToString("D4")}{Environment.NewLine}");

            // Prints game mode specific text and options. This typically is menus from commands, or states showing some information.
            tui.Append($"{RenderMode(windowMan)}{Environment.NewLine}");

            // Only print and accept user input if there is a game mode and menu system to support it.
            if (GameSimulationApp.Instance.WindowManager.AcceptingInput)
            {
                // Allow user to see their input from buffer.
                tui.Append($"What is your choice? {GameSimulationApp.Instance.InputManager.InputBuffer}");
            }

            // Outputs the result of the string builder to TUI builder above.
            return tui.ToString();
        }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        /// <param name="windowMan">
        ///     Instance of the window manager so we don't have to get it ourselves and just use the same one
        ///     renderer is using.
        /// </param>
        private string RenderMode(WindowManagerMod windowMan)
        {
            // If TUI for active game mode is not null or empty then use it.
            var activeModeTUI = windowMan.ActiveMode?.OnRenderMode();
            if (!string.IsNullOrEmpty(activeModeTUI))
                return activeModeTUI;

            // Otherwise, display default message if null for mode.
            return windowMan.ActiveMode == null ? GAMEMODE_EMPTY_TUI : GAMEMODE_DEFAULT_TUI;
        }

        /// <summary>
        ///     Fired when the screen back buffer has changed from what is currently being shown, this forces a redraw.
        /// </summary>
        public event ScreenBufferDirty ScreenBufferDirtyEvent;

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            ScreenBuffer = string.Empty;
        }

        /// <summary>
        ///     Fired when the simulation ticks the module that it created inside of itself.
        /// </summary>
        public override void Tick()
        {
            // Get the current text user interface data from inheriting class.
            var tuiContent = OnRender();
            if (ScreenBuffer.Equals(tuiContent, StringComparison.InvariantCultureIgnoreCase))
                return;

            // Update the screen buffer with altered data.
            ScreenBuffer = tuiContent;
            ScreenBufferDirtyEvent?.Invoke(ScreenBuffer);
        }
    }
}