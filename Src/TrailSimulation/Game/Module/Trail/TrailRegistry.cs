// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/14/2015@3:41 AM

namespace TrailSimulation.Game
{
    using System.Collections.Generic;
    using Entity;

    /// <summary>
    ///     Complete trails the player can travel on using the simulation. Some are remakes and others new.
    /// </summary>
    public static class TrailRegistry
    {
        /// <summary>
        ///     Original Oregon trail which was in the 1986 Apple II version of the game.
        /// </summary>
        public static Trail OregonTrail
        {
            get
            {
                var oregonTrail = new Location[]
                {
                    new Settlement("Independence", Climate.Moderate),
                    new RiverCrossing("Kansas River Crossing", Climate.Continental, RiverOption.FerryOperator),
                    new RiverCrossing("Big Blue River Crossing", Climate.Continental),
                    new Settlement("Fort Kearney", Climate.Continental),
                    new Landmark("Chimney Rock", Climate.Moderate),
                    new Settlement("Fort Laramie", Climate.Moderate),
                    new Landmark("Independence Rock", Climate.Moderate),
                    new ForkInRoad("South Pass", Climate.Dry, new List<Location>
                    {
                        new Settlement("Fort Bridger", Climate.Dry),
                        new Landmark("Green River Shortcut", Climate.Dry)
                    }),
                    new RiverCrossing("Green River Crossing", Climate.Dry),
                    new Landmark("Soda Springs", Climate.Dry),
                    new Settlement("Fort Hall", Climate.Moderate),
                    new RiverCrossing("Snake River Crossing", Climate.Moderate, RiverOption.IndianGuide),
                    new Settlement("Fort Boise", Climate.Polar),
                    new ForkInRoad("Blue Mountains", Climate.Polar, new List<Location>
                    {
                        new Settlement("Fort Walla Walla", Climate.Polar),
                        new ForkInRoad("The Dalles", Climate.Polar, new List<Location>
                        {
                            new RiverCrossing("Columbia River", Climate.Moderate),
                            new TollRoad("Barlow Toll Road", Climate.Moderate)
                        })
                    }),
                    new Settlement("Oregon City", Climate.Moderate)
                };

                return new Trail(oregonTrail, 32, 164);
            }
        }

        /// <summary>
        ///     Debugging and testing trail that is used to quickly iterate over the different location types.
        /// </summary>
        public static Trail TestTrail
        {
            get
            {
                var testTrail = new Location[]
                {
                    new Settlement("Start Settlement", Climate.Moderate),
                    new Landmark("Landmark", Climate.Dry),
                    new ForkInRoad("Fork In Road", Climate.Polar, new List<Location>
                    {
                        new Settlement("Inserted Settlement", Climate.Polar),
                        new ForkInRoad("Inserted Fork In Road", Climate.Polar, new List<Location>
                        {
                            new RiverCrossing("Inserted River Crossing (default)", Climate.Moderate),
                            new TollRoad("Inserted Toll Road", Climate.Moderate)
                        })
                    }),
                    new RiverCrossing("River Crossing (with ferry)", Climate.Continental, RiverOption.FerryOperator),
                    new RiverCrossing("River Crossing (with Indian)", Climate.Continental, RiverOption.IndianGuide),
                    new Settlement("End Settlement", Climate.Moderate)
                };

                return new Trail(testTrail, 50, 100);
            }
        }

        /// <summary>
        ///     Debugging trail for quickly getting to the end of the game for points tabulation and high-score tests.
        /// </summary>
        public static Trail WinTrail
        {
            get
            {
                var testPoints = new Location[]
                {
                    new Settlement("Start Of Test", Climate.Moderate),
                    new Settlement("End Of Test", Climate.Dry)
                };

                return new Trail(testPoints, 50, 100);
            }
        }

        /// <summary>
        ///     Debugging trail for quickly drowning the player and killing them off so tombstones and epitaphs can be tested.
        /// </summary>
        public static Trail FailTrail
        {
            get
            {
                var testFail = new Location[]
                {
                    new Settlement("Start Of Test", Climate.Moderate),
                    new RiverCrossing("Wolf River Crossing", Climate.Continental, RiverOption.IndianGuide),
                    new RiverCrossing("Fox River Crossing", Climate.Moderate, RiverOption.IndianGuide),
                    new RiverCrossing("Otter River Crossing", Climate.Tropical, RiverOption.FerryOperator),
                    new RiverCrossing("Coyote River Crossing", Climate.Polar),
                    new RiverCrossing("Deer River Crossing", Climate.Continental),
                    new RiverCrossing("End Of Test", Climate.Dry)
                };

                return new Trail(testFail, 50, 100);
            }
        }
    }
}