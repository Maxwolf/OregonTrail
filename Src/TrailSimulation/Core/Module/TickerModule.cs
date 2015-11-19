using System;
using TrailSimulation.Widget;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Keeps track of the total number of ticks and keeps track of time relative to seconds with every pulse to the
    ///     system. The idea being that no matter what pulses our simulation like game engine, operating system, framework,
    ///     windows form, phone app, browser we can figure out what a second is from this.
    /// </summary>
    internal sealed class TickerModule : SimulationModule
    {
        /// <summary>
        ///     First tick of the simulation after startup.
        /// </summary>
        public delegate void FirstTimerTick();

        /// <summary>
        ///     Pulse from what ever is keeping the application alive since simulation has no inherit way of doing this itself, but
        ///     relies on this knowledge to obtain information about running time.
        /// </summary>
        public delegate void SystemTick();

        /// <summary>
        ///     Fired when the simulation has determined that enough pulses from the system ticks have occurred that a second in
        ///     real time has gone by and it will attempt to tick the internal logic of the game simulation that inherits above
        ///     this class.
        /// </summary>
        /// <param name="timerTickCount">Total number of seconds that have occurred since the simulation began.</param>
        public delegate void TimerTick(ulong timerTickCount);

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
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.Ticker" /> class.
        /// </summary>
        internal TickerModule()
        {
            // Date and time the simulation was started, which we use as benchmark for all future time passed.
            _lastTickTime = DateTime.UtcNow;
            _currentTickTime = DateTime.UtcNow;

            // Visual tick representations for other sub-systems.
            TotalSecondsTicked = 0;

            // Setup spinning pixel to show game is not thread locked.
            _spinningPixel = new SpinningPixel();
            TickPhase = _spinningPixel.Step();
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
        ///     Pulse from what ever is keeping the application alive since simulation has no inherit way of doing this itself, but
        ///     relies on this knowledge to obtain information about running time.
        /// </summary>
        public event SystemTick SystemTickEvent;

        /// <summary>
        ///     Allow any data structures to save themselves.
        /// </summary>
        public override void Destroy()
        {
            OnDestroy();
        }

        /// <summary>
        ///     Fired when the simulation has determined that enough pulses from the system ticks have occurred that a second in
        ///     real time has gone by and it will attempt to tick the internal logic of the game simulation that inherits above
        ///     this class.
        /// </summary>
        public event TimerTick SimulationTickEvent;

        /// <summary>
        ///     Tick the internal logic of the simulation. Mostly used for updating text user interface (TUI), and updating screen
        ///     buffer.
        /// </summary>
        public override void Tick()
        {
            OnSystemTick();
        }

        /// <summary>
        ///     First tick of the simulation after startup.
        /// </summary>
        public event FirstTimerTick FirstSimulationTickEvent;

        /// <summary>
        ///     Calculates the number of ticks that have elapsed since the beginning of simulation and to instantiate a TimeSpan
        ///     object. The TimeSpan object is then used to display the elapsed time using several other time intervals.
        /// </summary>
        private void OnSystemTick()
        {
            _currentTickTime = DateTime.UtcNow;
            var elapsedTicks = _currentTickTime.Ticks - _lastTickTime.Ticks;
            var elapsedSpan = new TimeSpan(elapsedTicks);
            SystemTickEvent?.Invoke();

            // Check if more than an entire second has gone by.
            if (!(elapsedSpan.TotalMilliseconds > TICK_INTERVAL))
                return;

            // Reset last tick time to current time for measuring towards next second tick.
            _lastTickTime = _currentTickTime;
            OnTick();
        }

        /// <summary>
        ///     Fired when the simulation is loaded and makes it very first tick using the internal timer mechanism keeping track
        ///     of ticks to keep track of seconds.
        /// </summary>
        private void OnFirstTick()
        {
            // Fire event that delegate subs will be able to get notification about.
            FirstSimulationTickEvent?.Invoke();
        }

        /// <summary>
        ///     Fired when the simulation is closing down and would like to offer other sub-systems a change to properly shut down
        ///     and or close and save any information they care about before the end comes.
        /// </summary>
        private void OnDestroy()
        {
            // Nothing to see here, move along...
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
                OnFirstTick();
            }

            // Visual representation of ticking for debugging purposes.
            TickPhase = _spinningPixel.Step();

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            SimulationTickEvent?.Invoke(TotalSecondsTicked);
        }
    }
}