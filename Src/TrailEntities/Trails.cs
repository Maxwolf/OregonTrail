using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public static class Trails
    {
        /// <summary>
        ///     Creates the original Oregon trail which was in the game this is cloning.
        /// </summary>
        public static ReadOnlyCollection<PointOfInterest> OregonTrail
        {
            get
            {
                var trail = new List<PointOfInterest>
                {
                    new SettlementPoint("Independence", 0, true, Stores.MattsStore),
                    new RiverPoint("Kansas River Crossing", 83),
                    new RiverPoint("Big Blue River Crossing", 119),
                    new SettlementPoint("Fort Kearney", 250, true, Stores.FortKearneyStore),
                    new LandmarkPoint("Chimney Rock", 86, true),
                    new SettlementPoint("Fort Laramie", 190, true, Stores.FortLaramieStore),
                    new LandmarkPoint("Independence Rock", 152, true),
                    new ForkInRoadPoint("South Pass", 219, new List<PointOfInterest>
                    {
                        new SettlementPoint("Fort Bridger", 162, true, Stores.FortBridgerStore),
                        new RiverPoint("Green River Shortcut", 144)
                    }),
                    new RiverPoint("Green River Crossing", 94),
                    new LandmarkPoint("Soda Springs", 57, true),
                    new SettlementPoint("Fort Hall", 182, true, Stores.FortHallStore),
                    new RiverPoint("Snake River Crossing", 114),
                    new SettlementPoint("Fort Boise", 94, true, Stores.FortBoiseStore),
                    new ForkInRoadPoint("Blue Mountains", 91, new List<PointOfInterest>
                    {
                        new SettlementPoint("Fort Walla Walla", 120, true, Stores.FortWallaWallaStore),
                        new LandmarkPoint("The Dalles", 146, true)
                    }),
                    new SettlementPoint("Oregon City", 85, false)
                };
                return trail.AsReadOnly();
            }
        }
    }
}