using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class LandmarkMode : GameMode<LandmarkCommands>, ILandmarkMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public LandmarkMode()
        {
            CanRest = true;
        }

        public bool CanRest { get; }

        public override ModeType ModeType
        {
            get { return ModeType.Landmark; }
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
        }
    }
}