using System;
using TrailSimulation.Game;
using TrailSimulation.Widget;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Base simulation application class object. This class should not be declared directly but inherited by actual
    ///     instance of game controller.
    /// </summary>
    public abstract class SimulationApp
    {
        /// <summary>
        ///     Determines if the dynamic menu system should show the command names or only numbers. If false then only numbers
        ///     will be shown.
        /// </summary>
        public const bool SHOW_COMMANDS = false;

        /// <summary>
        ///     Constant for the amount of time difference that should occur from last tick and current tick in milliseconds before
        ///     the simulation logic will be ticked.
        /// </summary>
        private const double TICK_INTERVAL = 1000.0d;

        /// <summary>
        ///     Time and date of latest system tick, used to measure total elapsed time and tick simulation after each second.
        /// </summary>
        private DateTime _currentTickTime;

        /// <summary>
        ///     Last known time the simulation was ticked with logic and all sub-systems. This is not the same as a system tick
        ///     which can happen hundreds of thousands of times a second or just a few, we only measure the difference in time on
        ///     them.
        /// </summary>
        private DateTime _lastTickTime;

        /// <summary>
        ///     Spinning character pixel.
        /// </summary>
        private SpinningPixel _spinningPixel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            // Date and time the simulation was started, which we use as benchmark for all future time passed.
            _lastTickTime = DateTime.UtcNow;
            _currentTickTime = DateTime.UtcNow;

            // Visual tick representations for other sub-systems.
            TotalSecondsTicked = 0;

            // Setup spinning pixel to show game is not thread locked.
            _spinningPixel = new SpinningPixel();
            TickPhase = _spinningPixel.Step();

            // Create modules needed for managing simulation.
            Random = new RandomModuleProduct();
            ModeManager = new ModeManagerModuleProduct();
            TextRender = new TextRenderingModuleProduct();

            // Input manager needs event hook for knowing when buffer is sent.
            InputManagerManager = new InputManagerModule();
        }

        /// <summary>
        ///     Shows the current status of the simulation visually as a spinning glyph, the purpose of which is to show that there
        ///     is no hang in the simulation or logic controllers and everything is moving along and waiting for input or
        ///     displaying something to user.
        /// </summary>
        internal string TickPhase { get; private set; }

        /// <summary>
        ///     Total number of ticks that have gone by from measuring system ticks, this means this measures the total number of
        ///     seconds that have gone by using the pulses and time dilation without the use of dirty times that spawn more
        ///     threads.
        /// </summary>
        private ulong TotalSecondsTicked { get; set; }

        /// <summary>
        ///     Used for rolling the virtual dice in the simulation to determine the outcome of various events.
        /// </summary>
        internal RandomModuleProduct Random { get; private set; }

        /// <summary>
        ///     Keeps track of the currently attached game mode, which one is active, and getting text user interface data.
        /// </summary>
        internal ModeManagerModuleProduct ModeManager { get; private set; }

        /// <summary>
        ///     Handles input from the users keyboard, holds an input buffer and will push it to the simulation when return key is
        ///     pressed.
        /// </summary>
        public InputManagerModule InputManagerManager { get; private set; }

        /// <summary>
        ///     Shows the current state of the simulation as text only interface (TUI). Uses default constants if the attached mode
        ///     or state does not override this functionality and it is ticked.
        /// </summary>
        public TextRenderingModuleProduct TextRender { get; private set; }

        /// <summary>
        ///     Calculates the number of ticks that have elapsed since the beginning of simulation and to instantiate a TimeSpan
        ///     object. The TimeSpan object is then used to display the elapsed time using several other time intervals.
        /// </summary>
        private void OnSystemTick()
        {
            _currentTickTime = DateTime.UtcNow;
            var elapsedTicks = _currentTickTime.Ticks - _lastTickTime.Ticks;
            var elapsedSpan = new TimeSpan(elapsedTicks);

            // Check if more than an entire second has gone by.
            if (!(elapsedSpan.TotalMilliseconds > TICK_INTERVAL))
                return;

            // Reset last tick time to current time for measuring towards next second tick.
            _lastTickTime = _currentTickTime;
            OnTick();
        }

        /// <summary>
        ///     Fired when an actual entire second of system ticks have gone by and we would like to try and tick the internal game
        ///     simulation logic and everything that comes from that which will move the progress of the simulation forward.
        /// </summary>
        private void OnTick()
        {
            // Increase the tick count.
            TotalSecondsTicked++;

            // Fire event for first tick when it occurs, and only then.
            if (TotalSecondsTicked == 1)
            {
                GameSimulationApp.Instance.OnFirstTick();
            }

            // Visual representation of ticking for debugging purposes.
            TickPhase = _spinningPixel.Step();
        }

        /// <summary>
        ///     Fired when the ticker receives the first system tick event.
        /// </summary>
        public abstract void OnFirstTick();

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public void Destroy()
        {
            // Allows game simulation above us to cleanup any data structures it cares about.
            OnBeforeDestroy();

            // Remove simulation presentation variables.
            _lastTickTime = DateTime.MinValue;
            _currentTickTime = DateTime.MinValue;
            TotalSecondsTicked = 0;
            _spinningPixel = null;
            TickPhase = string.Empty;

            // Remove simulation core modules.
            Random = null;
            ModeManager = null;
            TextRender = null;
            InputManagerManager = null;
        }

        /// <summary>
        ///     Called when simulation is about to destroy itself, but right before it actually does it.
        /// </summary>
        protected abstract void OnBeforeDestroy();

        /// <summary>
        ///     Fired so the simulation can use constant stream of system ticks to determine interval pulse of a second so modules
        ///     that want to be ticked consistently may do so regardless of incoming system ticks (so long as it is more than a
        ///     couple in a second, typically in host there will be hundreds of ticks per second).
        /// </summary>
        public virtual void Tick()
        {
            OnSystemTick();

            // Changes game mode and state when needed.
            ModeManager?.Tick();

            // Sends commands if queue has any.
            InputManagerManager?.Tick();

            // Back buffer for only sending text when changed.
            TextRender?.Tick();

            // Rolls virtual dice.
            Random?.Tick();
        }
    }
}