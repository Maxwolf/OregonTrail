using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Complete trails the player can travel on using the simulation. Some are remakes and others new.
    /// </summary>
    public static class TrailRegistry
    {
        /// <summary>
        ///     Creates the original Oregon trail which was in the game this is cloning.
        /// </summary>
        public static IEnumerable<Location> OregonTrail()
        {
            var trail = new HashSet<Location>
            {
                new Location("Independence", 0, StoreRegistry.MattsStore()),
                new River("Kansas River Crossing", 83),
                new River("Big Blue River Crossing", 119),
                new Location("Fort Kearney", 250, StoreRegistry.FortKearneyStore()),
                new Location("Chimney Rock", 86),
                new Location("Fort Laramie", 190, StoreRegistry.FortLaramieStore()),
                new Location("Independence Rock", 152),
                new ForkInRoad("South Pass", 219, new List<Location>
                {
                    new Location("Fort Bridger", 162, StoreRegistry.FortBridgerStore()),
                    new River("Green River Shortcut", 144)
                }),
                new River("Green River Crossing", 94),
                new Location("Soda Springs", 57),
                new Location("Fort Hall", 182, StoreRegistry.FortHallStore()),
                new River("Snake River Crossing", 114),
                new Location("Fort Boise", 94, StoreRegistry.FortBoiseStore()),
                new ForkInRoad("Blue Mountains", 91, new List<Location>
                {
                    new Location("Fort Walla Walla", 120, StoreRegistry.FortWallaWallaStore()),
                    new Location("The Dalles", 146)
                }),
                new Location("Oregon City", 85, null, false)
            };
            return trail;
        }
    }
}