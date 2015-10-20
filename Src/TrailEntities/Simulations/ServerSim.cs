using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Base simulation class that deals with ticks, time, named pipes, and game modes.
    /// </summary>
    public abstract class ServerSim : TickSim, ISimulation
    {
        /// <summary>
        ///     Server named pipe that processes active game mode logic and sends messages to client about commands and waits for
        ///     responses.
        /// </summary>
        private readonly ServerPipe _server;

        /// <summary>
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        private List<IMode> _modes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationHost" /> class.
        /// </summary>
        protected ServerSim()
        {
            // References all of the active game modes that need to be ticked.
            _modes = new List<IMode>();

            // Create new server pipe that will send messages to game controller clients.
            _server = new ServerPipe(this);
        }

        public IServerPipe Server
        {
            get { return _server; }
        }

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

        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;
        public event ModeChanged ModeChangedEvent;

        public void AddMode(ModeType mode)
        {
            // Create new mode, check if it is in mode list.
            var changeMode = OnModeChanging(mode);
            if (_modes.Contains(changeMode))
                return;

            _modes.Add(changeMode);
            ModeChangedEvent?.Invoke(changeMode.Mode);
        }

        public uint TotalClients
        {
            get { return (uint) _server.Clients.Count; }
        }

        protected abstract GameMode OnModeChanging(ModeType mode);

        protected override void OnFirstTick()
        {
            base.OnFirstTick();

            // Server processes game logic, client sends command for server to process. Together they ward off thread-locking.
            _server.Start();
        }

        public override void OnDestroy()
        {
            _server.Stop();
            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        protected override void OnTick()
        {
            TickPipes();
            TickModes();
        }

        /// <summary>
        ///     Pump messages from server to client about valid commands, and screen data.
        /// </summary>
        private void TickPipes()
        {
            _server?.TickPipe();
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