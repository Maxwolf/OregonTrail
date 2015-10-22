using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    public sealed class HuntMode : GameMode<HuntingCommands>, IHunt
    {
        public override ModeType Mode
        {
            get { return ModeType.Hunt; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        public void UseBullets(uint amount)
        {
            throw new NotImplementedException();
        }

        public void AddFood(uint amount)
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
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
    }
}