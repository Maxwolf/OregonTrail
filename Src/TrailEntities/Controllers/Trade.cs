using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    public abstract class Trade : ITrade
    {
        public TrailModeType Mode
        {
            get { throw new NotImplementedException(); }
        }

        public ITrailVehicle TrailVehicle
        {
            get { throw new NotImplementedException(); }
        }

        public ReadOnlyCollection<IItem> PossibleTrades
        {
            get { throw new NotImplementedException(); }
        }

        public void TradeAttempt(IItem item)
        {
            throw new NotImplementedException();
        }
    }
}