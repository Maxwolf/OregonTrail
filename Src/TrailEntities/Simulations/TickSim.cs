using System;
using System.Timers;
using TrailCommon;

namespace TrailEntities
{
    public abstract class TickSim : ITick
    {
        /// <summary>
        ///     Random singleton with some extra methods for making life easy when dealing with simulations.
        /// </summary>
        private readonly Randomizer _random;

        /// <summary>
        ///     Non-threaded timer that waits for configured amount of time and fires events reliably.
        /// </summary>
        private readonly Timer _tick;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TickSimulation" /> class.
        /// </summary>
        protected TickSim()
        {
            _random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);

            TotalTicks = 0;
            TickPhase = "*";

            // Create timer for every second, enabled by default, hook elapsed event.
            _tick = new Timer(1000);
            _tick.Elapsed += OnTickFired;

            // Do not allow timer to automatically tick, this prevents it spawning multiple threads, enable the timer.
            _tick.AutoReset = false;
            _tick.Enabled = true;
        }

        public string TickPhase { get; private set; }

        public bool IsClosing { get; private set; }

        public Randomizer Random
        {
            get { return _random; }
        }

        public uint TotalTicks { get; private set; }

        public event TrailCommon.TickSim TickEvent;
        public event FirstTick FirstTickEvent;

        public void CloseSimulation()
        {
            // Allow any data structures to save themselves.
            IsClosing = true;
            OnDestroy();
        }

        private void OnTickFired(object Sender, ElapsedEventArgs e)
        {
            Tick();

            // Allow the timer to tick again now that we have finished working.
            _tick.Enabled = true;
        }

        private void Tick()
        {
            // Increase the tick count.
            TotalTicks++;

            if (TotalTicks == 1)
            {
                OnFirstTick();
            }

            TickPhase = TickVisualizer(TickPhase);

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            TickEvent?.Invoke(TotalTicks);
            OnTick();
        }

        protected virtual void OnFirstTick()
        {
            // Fire event that delegate subs will be able to get notification about.
            FirstTickEvent?.Invoke();
        }

        /// <summary>
        ///     Used for showing player that simulation is ticking on main view.
        /// </summary>
        private static string TickVisualizer(string currentPhase)
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

        public abstract void OnDestroy();

        protected abstract void OnTick();
    }
}