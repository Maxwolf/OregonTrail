using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public sealed class RiverCrossingMode : GameMode, IRiverCrossing
    {
        private uint _depth;
        private uint _ferryCost;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public RiverCrossingMode(IGameServer game) : base(game)
        {
            _depth = (uint) game.Random.Next(1, 20);
            _ferryCost = (uint) game.Random.Next(3, 8);
        }

        public uint Depth
        {
            get { return _depth; }
        }

        public uint FerryCost
        {
            get { return _ferryCost; }
        }

        public void CaulkVehicle()
        {
            throw new NotImplementedException();
        }

        public void Ford()
        {
            throw new NotImplementedException();
        }

        public void UseFerry()
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }

        public override ModeType Mode
        {
            get { return ModeType.RiverCrossing; }
        }

        public void CrossRiver()
        {
            throw new NotImplementedException();
        }
    }
}