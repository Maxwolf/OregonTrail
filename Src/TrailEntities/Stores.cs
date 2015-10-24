using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public static class Stores
    {
        /// <summary>
        ///     Matt's General Store is the first place in the game to buy supplies. This is where you stock up with everything
        ///     you need to start your journey on the trail. Matt's General Store is also the cheapest place (other than trading)
        ///     in the game to buy items.
        /// </summary>
        public static ReadOnlyCollection<Item> MattsGeneralStore
        {
            get
            {
                var _store = new List<Item>();
                //{ OxenItem(20, ), }
                return _store.AsReadOnly();
            }
        }
    }
}