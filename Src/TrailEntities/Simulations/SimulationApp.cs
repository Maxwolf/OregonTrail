using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using TrailCommon;

namespace TrailEntities
{
    public abstract class SimulationApp : ISimulation
    {
        private readonly ClientPipe _client;
        private readonly Randomizer _random;
        private readonly ServerPipe _server;
        private readonly SimulationType _simulationType;
        private readonly Timer _tickTimer;
        private List<IMode> _modes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        protected SimulationApp(SimulationType simulationType)
        {
            _simulationType = simulationType;
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

            switch (simulationType)
            {
                case SimulationType.Server:
                    _server = new ServerPipe(this);
                    break;
                case SimulationType.Client:
                    _client = new ClientPipe(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(simulationType), simulationType, null);
            }
        }

        public SimulationType SimulationType
        {
            get { return _simulationType; }
        }

        public IServerPipe Server
        {
            get { return _server; }
        }

        public IClientPipe Client
        {
            get { return _client; }
        }

        public string TickPhase { get; private set; }

        public void RemoveMode(ModeType mode)
        {
            // Ensure the mode exists as active mode.
            if (ActiveMode != null && ActiveMode.Mode != mode)
                return;

            // Ensure modes list contains active mode.
            if (!_modes.Contains(ActiveMode))
                return;

            // Remove the mode from list.
            _modes.Remove(ActiveMode);

            // Check if there are any modes after removal.
            if (ActiveMode != null)
                ModeChangedEvent?.Invoke(ActiveMode.Mode);
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
        public event FirstTick FirstTickEvent;
        public event ModeChanged ModeChangedEvent;

        public void AddMode(ModeType mode)
        {
            // Only servers can do this!
            if (_simulationType == SimulationType.Client)
                return;

            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(mode);
            if (_modes.Contains(changeMode))
                return;

            _modes.Add(changeMode);
            ModeChangedEvent?.Invoke(changeMode.Mode);
        }

        public void CloseSimulation()
        {
            // Allow any data structures to save themselves.
            Console.WriteLine("Closing...");
            IsClosing = true;
            OnDestroy();
        }

        public uint TotalClients
        {
            get
            {
                switch (SimulationType)
                {
                    case SimulationType.Server:
                        return (uint) _server.Clients.Count;
                    case SimulationType.Client:
                        return 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

        protected virtual void OnFirstTick()
        {
            // Fire event that delegate subs will be able to get notification about.
            FirstTickEvent?.Invoke();

            // Server processes game logic, client sends command for server to process. Together they ward off thread-locking.
            switch (SimulationType)
            {
                case SimulationType.Server:
                    _server.Start();
                    break;
                case SimulationType.Client:
                    _client.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public virtual void OnDestroy()
        {
            // Destroy named pipe inter-process communication system.
            switch (SimulationType)
            {
                case SimulationType.Server:
                    _server.Stop();
                    break;
                case SimulationType.Client:
                    _client.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        protected virtual void OnTick()
        {
            TickPipes();
            TickModes();
        }

        /// <summary>
        ///     Pump messages from server to client about valid commands, and screen data.
        /// </summary>
        private void TickPipes()
        {
            switch (SimulationType)
            {
                case SimulationType.Server:
                    _server?.TickPipe();
                    break;
                case SimulationType.Client:
                    _client?.TickPipe();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Process top-most game mode logic.
        /// </summary>
        private void TickModes()
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