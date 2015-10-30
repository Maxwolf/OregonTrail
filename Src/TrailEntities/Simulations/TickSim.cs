using System;
using System.Timers;
using TrailCommon;

namespace TrailEntities
{
    public abstract class TickSim : ITick
    {
        /// <summary>
        ///     Non-threaded timer that waits for configured amount of time and fires events reliably.
        /// </summary>
        private Timer _tick;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TickSimulation" /> class.
        /// </summary>
        protected TickSim()
        {
            Random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);

            TotalTimerTicks = 0;
            TimerTickPhase = "*";

            // Create timer for every second, enabled by default, hook elapsed event.
            _tick = new Timer(1000);
            _tick.Elapsed += OnTimerTickFired;
            _tick.Enabled = false;

            // Do not allow timer to automatically tick, this prevents it spawning multiple threads, enable the timer.
            _tick.AutoReset = false;
            _tick.Enabled = true;
        }

        public uint TotalSystemTicks { get; private set; }

        public string TimerTickPhase { get; private set; }

        public event SystemTick SystemTickEvent;

        public virtual void Destroy()
        {
            // Allow any data structures to save themselves.
            IsClosing = true;
            OnDestroy();
        }

        public event TimerTick TimerTickEvent;

        /// <summary>
        ///     Determines if the simulation is currently in the process of shutting down and cleaning up any resources it was
        ///     using.
        /// </summary>
        public bool IsClosing { get; private set; }

        /// <summary>
        ///     Random singleton with some extra methods for making life easy when dealing with simulations.
        /// </summary>
        public Randomizer Random { get; private set; }

        public void SystemTick()
        {
            OnSystemTick();
        }

        public uint TotalTimerTicks { get; private set; }

        public event FirstTimerTick FirstTimerTickEvent;

        protected virtual void OnSystemTick()
        {
            SystemTickEvent?.Invoke(++TotalSystemTicks);
        }

        private void OnTimerTickFired(object Sender, ElapsedEventArgs e)
        {
            TimerTick();

            // Allow the timer to tick again now that we have finished working.
            if (_tick != null)
                _tick.Enabled = true;
        }

        private void TimerTick()
        {
            OnTimerTick();
        }

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

        protected virtual void OnDestroy()
        {
            _tick = null;
            Random = null;
        }

        protected virtual void OnTimerTick()
        {
            // Increase the tick count.
            TotalTimerTicks++;

            if (TotalTimerTicks == 1)
            {
                OnFirstTimerTick();
            }

            TimerTickPhase = TimerTickVisualizer(TimerTickPhase);

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            TimerTickEvent?.Invoke(TotalTimerTicks);
        }
    }
}