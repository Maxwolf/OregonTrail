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
                    new SettlementPoint("Independence", 0, Stores.MattsStore),
                    new RiverPoint("Kansas River Crossing", 83),
                    new RiverPoint("Big Blue River Crossing", 119),
                    new FortPoint("Fort Kearney", 250, Stores.FortKearneyStore),
                    new LandmarkPoint("Chimney Rock", 86),
                    new FortPoint("Fort Laramie", 190, Stores.FortLaramieStore),
                    new LandmarkPoint("Independence Rock", 152),
                    new ForkInRoadPoint("South Pass", 219, new List<PointOfInterest>
                    {
                        new FortPoint("Fort Bridger", 162, Stores.FortBridgerStore),
                        new RiverPoint("Green River Shortcut", 144)
                    }),
                    new RiverPoint("Green River Crossing", 94),
                    new LandmarkPoint("Soda Springs", 57),
                    new FortPoint("Fort Hall", 182, Stores.FortHallStore),
                    new RiverPoint("Snake River Crossing", 114),
                    new FortPoint("Fort Boise", 94, Stores.FortBoiseStore),
                    new ForkInRoadPoint("Blue Mountains", 91, new List<PointOfInterest>
                    {
                        new FortPoint("Fort Walla Walla", 120, Stores.FortWallaWallaStore),
                        new LandmarkPoint("The Dalles", 146)
                    }),
                    new SettlementPoint("Oregon City", 85)
                };
                return trail.AsReadOnly();
            }
        }
    }
}