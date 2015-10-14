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
        private List<IItem> _possibleTrades;
        private IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trade" /> class.
        /// </summary>
        protected Trade(IVehicle vehicle)
        {
            _possibleTrades = new List<IItem>();
            _vehicle = vehicle;
        }

        public ReadOnlyCollection<IItem> PossibleTrades
        {
            get { return new ReadOnlyCollection<IItem>(_possibleTrades); }
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public void TradeAttempt(IItem item)
        {
            throw new NotImplementedException();
        }
    }
}