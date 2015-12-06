using System.Collections.Generic;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Complete trails the player can travel on using the simulation. Some are remakes and others new.
    /// </summary>
    public static class TrailRegistry
    {
        /// <summary>
        ///     Creates the original Oregon trail which was in the 1986 Apple II version of the game.
        /// </summary>
        public static Trail OregonTrail
        {
            get
            {
                var oregonTrail = new[]
                {
                    new Location("Independence", LocationCategory.Settlement),
                    new Location("Kansas River Crossing", LocationCategory.RiverCrossing),
                    new Location("Big Blue River Crossing", LocationCategory.RiverCrossing),
                    new Location("Fort Kearney", LocationCategory.Settlement),
                    new Location("Chimney Rock", LocationCategory.Landmark),
                    new Location("Fort Laramie", LocationCategory.Settlement),
                    new Location("Independence Rock", LocationCategory.Landmark),
                    new Location("South Pass", LocationCategory.ForkInRoad, new List<Location>
                    {
                        new Location("Fort Bridger", LocationCategory.Settlement),
                        new Location("Green River Shortcut", LocationCategory.Landmark)
                    }),
                    new Location("Green River Crossing", LocationCategory.RiverCrossing),
                    new Location("Soda Springs", LocationCategory.Landmark),
                    new Location("Fort Hall", LocationCategory.Settlement),
                    new Location("Snake River Crossing", LocationCategory.RiverCrossing),
                    new Location("Fort Boise", LocationCategory.Settlement),
                    new Location("Blue Mountains", LocationCategory.ForkInRoad, new List<Location>
                    {
                        new Location("Fort Walla Walla", LocationCategory.Settlement),
                        new Location("The Dalles", LocationCategory.Landmark)
                    }),
                    new Location("Oregon City", LocationCategory.Settlement)
                };

                return new Trail(oregonTrail, 2000);
            }
        }
    }
}