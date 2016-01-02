// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    /// <summary>
    ///     Represents an offer that automatically generates itself when constructor is called. Randomly selects a want, and
    ///     then a offer both of which are simulation items. Depending on the inventory of the vehicle this may or may not be
    ///     possible depending on totals.
    /// </summary>
    public sealed class TradeOffer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.TradeOffer" /> class.
        /// </summary>
        public TradeOffer()
        {
            // Select a random item default inventory might have which the emigrant wants.
            WantedItem = Vehicle.CreateRandomItem();

            // Select random item from default inventory which the emigrant offers up in exchange.
            OfferedItem = Vehicle.CreateRandomItem();
        }

        /// <summary>
        ///     Wanted item from the players vehicle inventory in order to get the offered item.
        /// </summary>
        public SimItem WantedItem { get; }

        /// <summary>
        ///     Offers up an item in exchange for the traders wanted item.
        /// </summary>
        public SimItem OfferedItem { get; }
    }
}