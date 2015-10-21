using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Base simulation class that deals with ticks, time, named pipes, and game modes.
    /// </summary>
    public abstract class SimulationApp : TickSim, ISimulation
    {
        /// <summary>
        ///     Current list of all game modes, only the last one added gets ticked this is so game modes can attach things on-top
        ///     of themselves like stores and trades.
        /// </summary>
        private List<IMode> _modes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationHost" /> class.
        /// </summary>
        protected SimulationApp()
        {
            // References all of the active game modes that need to be ticked.
            _modes = new List<IMode>();
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
            // TODO: Remove all modes and attach travel mode, player is at first trail point.
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

        protected abstract GameMode OnModeChanging(ModeType mode);

        public override void OnDestroy()
        {
            base.OnDestroy();

            _modes.Clear();
            EndgameEvent?.Invoke();
        }

        protected override void OnTick()
        {
            TickModes();
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