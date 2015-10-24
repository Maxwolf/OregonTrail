using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public static class Trails
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
                    new SettlementPoint("Independence", 0, true),
                    new RiverPoint("Kansas River Crossing", 83),
                    new RiverPoint("Big Blue River Crossing", 119),
                    new SettlementPoint("Fort Kearney", 250, true),
                    new LandmarkPoint("Chimney Rock", 86, true),
                    new SettlementPoint("Fort Laramie", 190, true),
                    new LandmarkPoint("Independence Rock", 152, true),
                    new ForkInRoadPoint("South Pass", 219, new List<PointOfInterest>
                    {
                        new SettlementPoint("Fort Bridger", 162, true),
                        new RiverPoint("Green River Shortcut", 144)
                    }),
                    new RiverPoint("Green River Crossing", 94),
                    new LandmarkPoint("Soda Springs", 57, true),
                    new SettlementPoint("Fort Hall", 182, true),
                    new RiverPoint("Snake River Crossing", 114),
                    new SettlementPoint("Fort Boise", 94, true),
                    new ForkInRoadPoint("Blue Mountains", 91, new List<PointOfInterest>
                    {
                        new SettlementPoint("Fort Walla Walla", 120, true),
                        new LandmarkPoint("The Dalles", 146, true)
                    }),
                    new SettlementPoint("Oregon City", 85, false)
                };
                return _trail.AsReadOnly();
            }
        }
    }
}