using System;
using System.Timers;
using TrailCommon;

namespace TrailEntities
{
    public abstract class SimulationApp : ISimulation
    {
        private Randomizer _random;
        private GameMode _currentMode;
        private Timer _tickTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp()
        {
            _random = new Randomizer((int) DateTime.Now.Ticks & 0x0000FFF);
            TotalTicks = 0;
            TickPhase = "*";
            _currentMode = null;

            // Create timer for every second, enabled by default, hook elapsed event.
            _tickTimer = new Timer(1000);
            _tickTimer.Elapsed += OnTickTimerFired;

            // Do not allow timer to automatically tick, this prevents it spawning multiple threads, enable the timer.
            _tickTimer.AutoReset = false;
            _tickTimer.Enabled = true;
        }

        public GameMode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (_currentMode == value)
                    return;

                _currentMode = value;
                ModeChangedEvent?.Invoke(_currentMode.Mode);
            }
        }

        public static SimulationApp Instance { get; private set; }

        public string TickPhase { get; private set; }

        public abstract void ChooseProfession();

        public abstract void BuyInitialItems();

        public abstract void ChooseNames();

        public void StartGame()
        {
            NewgameEvent?.Invoke();
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

        private void OnTickTimerFired(object Sender, ElapsedEventArgs e)
        {
            Tick();

            // Allow the timer to tick again now that we have finished working.
            _tickTimer.Enabled = true;
        }

        private void Tick()
        {
            // We do not tick if there is no instance associated with it.
            if (Instance == null)
                throw new InvalidOperationException("Attempted to tick game initializer when instance is null!");

            // Increase the tick count.
            TotalTicks++;

            TickPhase = TickVisualizer(TickPhase);

            // Fire tick event for any subscribers to see and overrides for inheriting classes.
            TickEvent?.Invoke(TotalTicks);
            OnTick();
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

        public static void Create(SimulationApp gameInstance)
        {
            // Complain if the simulation app has already been created.
            if (Instance != null)
                throw new InvalidOperationException(
                    "Cannot create new simulation app when instance is not null, please call destroy before creating new simulation app instance.");

            Instance = gameInstance;
        }

        protected static void Destroy()
        {
            // Complain if destroy was awakened for no reason.
            if (Instance == null)
                throw new InvalidOperationException("Unable to destroy game manager, it has not been created yet!");

            // Allow any data structures to save themselves.
            Console.WriteLine("Closing...");
            Instance.OnDestroy();

            // Actually destroy the instance and close the program.
            Instance = null;
        }

        protected virtual void OnDestroy()
        {
            _currentMode = null;
            EndgameEvent?.Invoke();
        }

        protected abstract void OnTick();
    }
}