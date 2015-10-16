using System;
using TrailCommon;

namespace TrailGame
{
    public abstract class SimulationApp : ISimulation
    {
        private Random _random = new Random();
        private string _tickPhase = "*";
        private uint _totalTicks = 0;

        public static SimulationApp Instance { get; private set; }

        public Random Random
        {
            get { return _random; }
        }

        public string TickPhase
        {
            get { return _tickPhase; }
        }

        public void ChooseProfession()
        {
        }

        public void BuyInitialItems()
        {
        }

        public void ChooseNames()
        {
        }

        public void StartGame()
        {
            NewgameEvent?.Invoke();
        }

        public uint TotalTicks
        {
            get { return _totalTicks; }
        }

        public void SetMode(ITrailMode mode)
        {
            ModeChangedEvent?.Invoke(mode.Mode);
        }

        public void Tick()
        {
            // We do not tick if there is no instance associated with it.
            if (Instance == null)
                throw new InvalidOperationException("Attempted to tick game initializer when instance is null!");

            // Increase the tick count.
            _totalTicks++;

            _tickPhase = TickVisualizer(_tickPhase);

            // Fire tick event for any subscribers to see.
            TickEvent?.Invoke(_totalTicks);

            OnTick();
        }

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event ModeChanged ModeChangedEvent;
        public event TickTimeHandler TickEvent;

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
            EndgameEvent?.Invoke();
        }

        protected virtual void OnTick()
        {
        }
    }
}