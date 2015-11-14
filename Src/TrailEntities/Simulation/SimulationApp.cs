namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Base simulation application class object. This class should not be declared directly but inherited by actual
    ///     instance of game controller.
    /// </summary>
    public abstract class SimulationApp
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            // TickerMod module allows us to convert system tick pulses in steady stream of seconds.
            TickerMod = new TickerMod();
            TickerMod.FirstSimulationTickEvent += Ticker_FirstSimulationTickEvent;
            TickerMod.SimulationTickEvent += Ticker_SimulationTickEvent;
            TickerMod.SystemTickEvent += Ticker_SystemTickEvent;

            // Create modules needed for managing simulation.
            Random = new RandomizerMod();
            WindowManagerMod = new WindowManagerMod();
            TextRenderMod = new TextRenderMod();

            // Input manager needs event hook for knowing when buffer is sent.
            InputManagerMod = new InputManagerMod();
            InputManagerMod.InputManagerSendCommandEvent += InputManager_InputManagerSendCommandEvent;
        }

        /// <summary>
        ///     Keeps track of how many times the underlying system ticks, uses this data to create pulses of one second for the
        ///     simulation to sync itself to from any number of input ticks.
        /// </summary>
        internal TickerMod TickerMod { get; private set; }

        /// <summary>
        ///     Used for rolling the virtual dice in the simulation to determine the outcome of various events.
        /// </summary>
        internal RandomizerMod Random { get; private set; }

        /// <summary>
        ///     Keeps track of the currently attached game mode, which one is active, and getting text user interface data.
        /// </summary>
        internal WindowManagerMod WindowManagerMod { get; private set; }

        /// <summary>
        ///     Handles input from the users keyboard, holds an input buffer and will push it to the simulation when return key is
        ///     pressed.
        /// </summary>
        public InputManagerMod InputManagerMod { get; private set; }

        /// <summary>
        ///     Shows the current state of the simulation as text only interface (TUI). Uses default constants if the attached mode
        ///     or state does not override this functionality and it is ticked.
        /// </summary>
        public TextRenderMod TextRenderMod { get; private set; }

        /// <summary>
        ///     Fired when the input manager wants to send a command to the currently running game simulation.
        /// </summary>
        /// <param name="command">Command that wants to be passed into active game mode.</param>
        private void InputManager_InputManagerSendCommandEvent(string command)
        {
            WindowManagerMod.ActiveMode?.SendCommand(command);
        }

        /// <summary>
        ///     Fired when the underlying system, engine, potato ticks the simulation.
        /// </summary>
        private void Ticker_SystemTickEvent()
        {
            // Back buffer for only sending text when changed.
            TextRenderMod?.Tick();

            // Rolls virtual dice.
            Random?.Tick();
        }

        /// <summary>
        ///     Fired when the simulation deems a second has gone by from the averaged stream of system ticks.
        /// </summary>
        /// <param name="simTicks">Total number of seconds that have passed by.</param>
        private void Ticker_SimulationTickEvent(ulong simTicks)
        {
            // Sends commands if queue has any.
            InputManagerMod?.Tick();

            // Changes game mode and state when needed.
            WindowManagerMod?.Tick();
        }

        /// <summary>
        ///     Fired on the first actual simulation tick, this would be a system tick but helps us initialize the plumping of the
        ///     simulation.
        /// </summary>
        private void Ticker_FirstSimulationTickEvent()
        {
            OnFirstTick();
        }

        /// <summary>
        ///     Fired when the ticker receives the first system tick event.
        /// </summary>
        protected abstract void OnFirstTick();

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public void Destroy()
        {
            // TODO: Replace with attribute and reflection based initialization for simulation modules.
            OnBeforeDestroy();

            // Destroy window manager.
            WindowManagerMod.Destroy();
            WindowManagerMod = null;

            // Destroy input manager.
            InputManagerMod.InputManagerSendCommandEvent -= InputManager_InputManagerSendCommandEvent;
            InputManagerMod.Destroy();
            InputManagerMod = null;

            // Destroy text renderer.
            TextRenderMod.Destroy();
            TextRenderMod = null;

            // Destroy the ticker.
            TickerMod.FirstSimulationTickEvent -= Ticker_FirstSimulationTickEvent;
            TickerMod.SimulationTickEvent -= Ticker_SimulationTickEvent;
            TickerMod.SystemTickEvent -= Ticker_SystemTickEvent;
            TickerMod.Destroy();
            TickerMod = null;

            // Destroy the randomizer.
            Random.Destroy();
            Random = null;
        }

        /// <summary>
        ///     Called when simulation is about to destroy itself, but right before it actually does it.
        /// </summary>
        protected abstract void OnBeforeDestroy();

        /// <summary>
        ///     Called by external forces from the simulation such as underlying operating system, game engine, potato, etc.
        /// </summary>
        public void Tick()
        {
            // Converts pulses from OS into stream of seconds.
            TickerMod.Tick();
        }
    }
}