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
        public static Trail OregonTrail()
        {
            var oregonTrail = new[]
            {
                new Location("Independence", true),
                new River("Kansas River Crossing"),
                new River("Big Blue River Crossing"),
                new Location("Fort Kearney", true),
                new Location("Chimney Rock"),
                new Location("Fort Laramie", true),
                new Location("Independence Rock"),
                new ForkInRoad("South Pass", new List<Location>
                {
                    new Location("Fort Bridger", true),
                    new River("Green River Shortcut")
                }),
                new River("Green River Crossing"),
                new Location("Soda Springs"),
                new Location("Fort Hall", true),
                new River("Snake River Crossing"),
                new Location("Fort Boise", true),
                new ForkInRoad("Blue Mountains", new List<Location>
                {
                    new Location("Fort Walla Walla", true),
                    new Location("The Dalles")
                }),
                new Location("Oregon City")
            };

            return new Trail(oregonTrail, 2000);
        }
    }
}