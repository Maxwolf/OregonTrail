// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:38 PM

using System;
using System.Text;

namespace OregonTrailDotNet.WolfCurses.Core
{
    /// <summary>
    ///     Provides base functionality for rendering out the simulation state via text user interface (TUI). This class has no
    ///     idea about how other modules work and only serves to query them for string data which will be compiled into a
    ///     console only view of the simulation which is intended to be the lowest level of visualization but theoretically
    ///     anything could be a renderer for the simulation.
    /// </summary>
    public class SceneGraph : Module.Module
    {
        /// <summary>
        ///     Fired when the screen back buffer has changed from what is currently being shown, this forces a redraw.
        /// </summary>
        public delegate void ScreenBufferDirty(string tuiContent);

        /// <summary>
        ///     Default string used when game Windows has nothing better to say.
        /// </summary>
        private const string GAMEMODE_DEFAULT_TUI = "[DEFAULT WINDOW TEXT]";

        /// <summary>
        ///     Default string used when there are no game modes at all.
        /// </summary>
        private const string GAMEMODE_EMPTY_TUI = "[NO WINDOW ATTACHED]";

        /// <summary>
        ///     Reference to simulation that is controlling the text user interface and actually filling the screen buffer with
        ///     data.
        /// </summary>
        private readonly SimulationApp _simUnit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SceneGraph" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the window manager.</param>
        public SceneGraph(SimulationApp simUnit)
        {
            _simUnit = simUnit;
            ScreenBuffer = string.Empty;
        }

        /// <summary>
        ///     Holds the last known representation of the game simulation and current Windows text user interface, only pushes
        ///     update
        ///     when a change occurs.
        /// </summary>
        private string ScreenBuffer { get; set; }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            ScreenBuffer = string.Empty;
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
        public override void OnTick(bool systemTick, bool skipDay = false)
        {
            // GetModule the current text user interface data from inheriting class.
            var tuiContent = OnRender();
            if (ScreenBuffer.Equals(tuiContent, StringComparison.OrdinalIgnoreCase))
                return;

            // Update the screen buffer with altered data.
            ScreenBuffer = tuiContent;
            ScreenBufferDirtyEvent?.Invoke(ScreenBuffer);
        }

        /// <summary>
        ///     Prints game Windows specific text and options.
        /// </summary>
        /// <returns>
        ///     The text user interface that is the game simulation.<see cref="string" />.
        /// </returns>
        private string OnRender()
        {
            // Spinning ticker that shows activity, lets us know if application hangs or freezes.
            var tui = new StringBuilder();
            tui.Append($"[ {_simUnit.TickPhase} ] - ");

            // Keeps track of active Windows name and active Windows current state name for debugging purposes.
            tui.Append(_simUnit.WindowManager.FocusedWindow?.CurrentForm != null
                ? $"Window({_simUnit.WindowManager.Count}): {_simUnit.WindowManager.FocusedWindow}({_simUnit.WindowManager.FocusedWindow.CurrentForm}) - "
                : $"Window({_simUnit.WindowManager.Count}): {_simUnit.WindowManager.FocusedWindow}() - ");

            // Allows the implementing simulation to control text before window is rendered out.
            tui.Append(_simUnit.OnPreRender());

            // Prints game Windows specific text and options. This typically is menus from commands, or states showing some information.
            tui.Append($"{RenderWindow()}{Environment.NewLine}");

            if (_simUnit.WindowManager.AcceptingInput)
            {
                // Allow user to see their input from buffer.
                tui.Append($"What is your choice? {_simUnit.InputManager.InputBuffer}");
            }

            // Outputs the result of the string builder to TUI builder above.
            return tui.ToString();
        }

        /// <summary>Prints game Windows specific text and options.</summary>
        /// <returns>The current window text to be rendered out.<see cref="string" />.</returns>
        private string RenderWindow()
        {
            // If TUI for active game Windows is not null or empty then use it.
            var activeWindowText = _simUnit.WindowManager.FocusedWindow?.OnRenderWindow();
            if (!string.IsNullOrEmpty(activeWindowText) && !string.IsNullOrWhiteSpace(activeWindowText))
                return activeWindowText;

            // Otherwise, display default message if null for Windows.
            return _simUnit.WindowManager.FocusedWindow == null ? GAMEMODE_EMPTY_TUI : GAMEMODE_DEFAULT_TUI;
        }

        /// <summary>
        ///     Fired when the screen back buffer has changed from what is currently being shown, this forces a redraw.
        /// </summary>
        public event ScreenBufferDirty ScreenBufferDirtyEvent;

        /// <summary>
        ///     Removes any and all data from the text user interface renderer.
        /// </summary>
        public void Clear()
        {
            ScreenBuffer = string.Empty;
        }
    }
}