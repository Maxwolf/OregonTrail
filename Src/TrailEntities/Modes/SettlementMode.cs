using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class SettlementMode : GameMode<SettlementCommands>, ISettlementMode
    {
        private readonly bool _canRest;
        private readonly IStoreMode _storeMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public SettlementMode()
        {
            _canRest = true;
            _storeMode = new StoreMode();
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.Settlement; }
        }

        public bool CanRest
        {
            get { return _canRest; }
        }

        public IStoreMode StoreMode
        {
            get { return _storeMode; }
        }

        public void GoToStore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Called by the active game mode when the text user interface is called. This will create a string builder with all
        ///     the data and commands that represent the concrete handler for this game mode.
        /// </summary>
        protected override string OnGetModeTUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            throw new NotImplementedException();
        }
    }
}