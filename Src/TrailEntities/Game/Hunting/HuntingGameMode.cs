using System;
using TrailEntities.Mode;

namespace TrailEntities.Game.Hunting
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    public sealed class HuntingGameMode : ModeProduct
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public HuntingGameMode() : base(true)
        {
        }

        public override GameMode ModeType
        {
            get { return GameMode.Hunt; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game gameMode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }

        public void UseBullets(int amount)
        {
            throw new NotImplementedException();
        }

        public void AddFood(int amount)
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }
    }
}