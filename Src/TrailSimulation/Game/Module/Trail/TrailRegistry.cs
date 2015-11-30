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
                new Location("Kansas River Crossing", false, true),
                new Location("Big Blue River Crossing", false, true),
                new Location("Fort Kearney", true),
                new Location("Chimney Rock"),
                new Location("Fort Laramie", true),
                new Location("Independence Rock"),
                new Location("South Pass", false, false, new List<Location>
                {
                    new Location("Fort Bridger", true),
                    new Location("Green River Shortcut", false, true)
                }),
                new Location("Green River Crossing", false, true),
                new Location("Soda Springs"),
                new Location("Fort Hall", true),
                new Location("Snake River Crossing", false, true),
                new Location("Fort Boise", true),
                new Location("Blue Mountains", false, false, new List<Location>
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