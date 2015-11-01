using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Keeps track of the total number of ticks and keeps track of time relative to seconds with every pulse to the
    ///     system. The idea being that no matter what pulses our simulation like game engine, operating system, framework,
    ///     windows form, phone app, browser we can figure out what a second is from this.
    /// </summary>
    public abstract class TickSim
    {
        /// <summary>
        ///     First tick of the simulation after startup.
        /// </summary>
        public delegate void FirstTimerTick();

        /// <summary>
        ///     Pulse from what ever is keeping the application alive since simulation has no inherit way of doing this itself, but
        ///     relies on this knowledge to obtain information about running time.
        /// </summary>
        /// <param name="systemTickCount">Total number of ticks that have occurred in the entire simulation in lifetime.</param>
        public delegate void SystemTick(ulong systemTickCount);

        /// <summary>
        ///     Fired when the simulation has determined that enough pulses from the system ticks have occurred that a second in
        ///     real time has gone by and it will attempt to tick the internal logic of the game simulation that inherits above
        ///     this class.
        /// </summary>
        /// <param name="timerTickCount">Total number of seconds that have occurred since the simulation began.</param>
        public delegate void TimerTick(ulong timerTickCount);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TickSim" /> class.
        /// </summary>
        protected TickSim()
        {
            Random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);

            TotalSecondsTicked = 0;
            TimerTickPhase = "*";
        }

        /// <summary>
        ///     Total number of pulses from the underlying application that is keeping the context open for the rest of the
        ///     simulation since it has no way of thread locking or doing any blocking calls itself since that is against the
        ///     design protocols of this project. Ticks should be able to come from ANYTHING and it will be able to determine how
        ///     long a second is from this.
        /// </summary>
        private ulong TotalSystemTicks { get; set; }

        /// <summary>
        ///     Shows the current status of the simulation visually as a spinning glyph, the purpose of which is to show that there
        ///     is no hang in the simulation or logic controllers and everything is moving along and waiting for input or
        ///     displaying something to user.
        /// </summary>
        protected string TimerTickPhase { get; private set; }

        /// <summary>
        ///     Determines if the simulation is currently in the process of shutting down and cleaning up any resources it was
        ///     using.
        /// </summary>
        private bool IsClosing { get; set; }

        /// <summary>
        ///     Random singleton with some extra methods for making life easy when dealing with simulations.
        /// </summary>
        public Randomizer Random { get; private set; }

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
        public void Destroy()
        {
            IsClosing = true;
            OnDestroy();
        }

        /// <summary>
        ///     Fired when the simulation has determined that enough pulses from the system ticks have occurred that a second in
        ///     real time has gone by and it will attempt to tick the internal logic of the game simulation that inherits above
        ///     this class.
        /// </summary>
        public event TimerTick TimerTickEvent;

        /// <summary>
        ///     Tick the internal logic of the simulation. Mostly used for updating text user interface (TUI), and updating screen
        ///     buffer.
        /// </summary>
        public void TickSystem()
        {
            OnSystemTick();
        }

        /// <summary>
        ///     First tick of the simulation after startup.
        /// </summary>
        public event FirstTimerTick FirstTimerTickEvent;

        /// <summary>
        ///     Fired when there is a pulse from the underlying application that is keeping the program open and alive since the
        ///     simulation has no inherit way of doing that and will not block calls or threads on parent application.
        /// </summary>
        protected virtual void OnSystemTick()
        {
            SystemTickEvent?.Invoke(++TotalSystemTicks);
        }

        /// <summary>
        ///     Fired when the simulation is loaded and makes it very first tick using the internal timer mechanism keeping track
        ///     of ticks to keep track of seconds.
        /// </summary>
        protected virtual void OnFirstTimerTick()
        {
            // Fire event that delegate subs will be able to get notification about.
            FirstTimerTickEvent?.Invoke();
        }

        /// <summary>
        ///     Used for showing player that simulation is ticking on main view.
        /// </summary>
        private static string TimerTickVisualizer(string currentPhase)
        {
            switch (currentPhase)
            {
                case @"*":
                    return @"|";
                case @"|":
                    return @"/";
                case @"/":
                    return @"-";
                case @"-":
                    return @"\";
                case @"\":
                    return @"|";
                default:
                    return "*";
            }
        }

        /// <summary>
        ///     Fired when the simulation is closing down and would like to offer other sub-systems a change to properly shut down
        ///     and or close and save any information they care about before the end comes.
        /// </summary>
        protected virtual void OnDestroy()
        {
            Random = null;
        }

        /// <summary>
        ///     Fired when an actual entire second of system ticks have gone by and we would like to try and tick the internal game
        ///     simulation logic and everything that comes from that which will move the progress of the simulation forward.
        /// </summary>
        protected virtual void OnTimerTick()
        {
            // Increase the tick count.
            TotalSecondsTicked++;

            if (TotalSecondsTicked == 1)
            {
                OnFirstTimerTick();
            }

            TimerTickPhase = TimerTickVisualizer(TimerTickPhase);

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            TimerTickEvent?.Invoke(TotalSecondsTicked);
        }
    }
}