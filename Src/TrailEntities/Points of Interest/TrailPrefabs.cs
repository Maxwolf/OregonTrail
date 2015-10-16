using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public static class TrailPrefabs
    {
        /// <summary>
        ///     Creates the default Oregon trail which was in the original game.
        /// </summary>
        public static ReadOnlyCollection<PointOfInterest> OregonTrail
        {
            get
            {
                var _trail = new List<PointOfInterest>
                {
                    new Settlement("Independence", 0, true),
                    new RiverCrossing("Kansas River Crossing", 83),
                    new RiverCrossing("Big Blue River Crossing", 119),
                    new Settlement("Fort Kearney", 250, true),
                    new Landmark("Chimney Rock", 86, true),
                    new Settlement("Fort Laramie", 190, true),
                    new Landmark("Independence Rock", 152, true),
                    new ForkInRoad("South Pass", 219, true, new List<PointOfInterest>
                    {
                        new Settlement("Fort Bridger", 162, true),
                        new RiverCrossing("Green River Shortcut", 144)
                    }),
                    new RiverCrossing("Green River Crossing", 94),
                    new Landmark("Soda Springs", 57, true),
                    new Settlement("Fort Hall", 182, true),
                    new RiverCrossing("Snake River Crossing", 114),
                    new Settlement("Fort Boise", 94, true),
                    new ForkInRoad("Blue Mountains", 91, true, new List<PointOfInterest>
                    {
                        new Settlement("Fort Walla Walla", 120, true),
                        new Landmark("The Dalles", 146, true)
                    }),
                    new Settlement("Oregon City", 85, false)
                };
                return new ReadOnlyCollection<PointOfInterest>(_trail);
            }
        }
    }
}