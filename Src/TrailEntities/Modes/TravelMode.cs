using System;
using TrailCommon;

namespace TrailEntities
{
    public class TravelMode : GameMode, ITravel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public TravelMode(IGameSimulation game) : base(game)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Travel; }
        }

        public void Hunt()
        {
            throw new NotImplementedException();
        }

        public void Rest()
        {
            throw new NotImplementedException();
        }

        public void Trade()
        {
            throw new NotImplementedException();
        }
    }
}