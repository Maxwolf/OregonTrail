using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Timers;
using TrailCommon;
using Timer = System.Timers.Timer;

namespace TrailEntities
{
    public abstract class SimulationApp : ISimulation
    {
        /// <summary>
        ///     Used in the pipe system to help identify the correct address, this should not be changed unless you know what you
        ///     are doing.
        /// </summary>
        protected const string SimulationName = ".";

        /// <summary>
        ///     To wait till a response is received for a request and THEN proceed
        /// </summary>
        protected readonly AutoResetEvent _waitForResponse = new AutoResetEvent(false);

        /// <summary>
        ///     Equivalent to receiving a "quit" on the console
        /// </summary>
        protected bool _cancelRequested;

        protected string _currentId;

        private List<IMode> _modes;
        private Randomizer _random;
        private Timer _tickTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            _random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);
            TotalTicks = 0;
            TickPhase = "*";
            _modes = new List<IMode>();

            // Create timer for every second, enabled by default, hook elapsed event.
            _tickTimer = new Timer(1000);
            _tickTimer.Elapsed += OnTickTimerFired;

            // Do not allow timer to automatically tick, this prevents it spawning multiple threads, enable the timer.
            _tickTimer.AutoReset = false;
            _tickTimer.Enabled = true;
        }

        public string TickPhase { get; private set; }

        public void RemoveMode(ModeType mode)
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            NewgameEvent?.Invoke();
        }

        public bool IsClosing { get; private set; }

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

        public ReadOnlyCollection<IMode> Modes
        {
            get { return new ReadOnlyCollection<IMode>(_modes); }
        }

        public Randomizer Random
        {
            get { return _random; }
        }

        public uint TotalTicks { get; private set; }

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event TickSim TickEvent;
        public event ModeChanged ModeChangedEvent;

        public void AddMode(ModeType mode)
        {
            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(mode);
            if (!_modes.Contains(changeMode))
            {
                _modes.Add(changeMode);
                ModeChangedEvent?.Invoke(changeMode.Mode);
            }
        }

        public void CloseSimulation()
        {
            // Allow any data structures to save themselves.
            Console.WriteLine("Closing...");
            IsClosing = true;
            OnDestroy();
        }

        protected abstract GameMode OnModeChanging(ModeType mode);

        private void OnTickTimerFired(object Sender, ElapsedEventArgs e)
        {
            Tick();

            // Allow the timer to tick again now that we have finished working.
            _tickTimer.Enabled = true;
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

        protected abstract void OnFirstTick();

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

        public virtual void OnDestroy()
        {
            _cancelRequested = true;
            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        protected virtual void OnTick()
        {
            // Only tick if there are modes to tick.
            if (_modes.Count <= 0)
                return;

            // Only top-most game mode gets ticking action.
            var lastMode = _modes[_modes.Count - 1];
            lastMode?.TickMode();
        }
    }
}