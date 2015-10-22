using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class LandmarkMode : GameMode<LandmarkCommands>, ILandmarkPoint
    {
        private readonly bool _canRest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public LandmarkMode()
        {
            _canRest = true;
        }

        public override ModeType Mode
        {
            get { return ModeType.Landmark; }
        }

        public bool CanRest
        {
            get { return _canRest; }
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
    }
}