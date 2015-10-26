using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class SettlementMode : GameMode<SettlementCommands>, ISettlementMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public SettlementMode()
        {
            CanRest = true;
            StoreMode = new StoreMode();
        }

        public bool CanRest { get; }

        public IStoreMode StoreMode { get; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Settlement; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        public void GoToStore()
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