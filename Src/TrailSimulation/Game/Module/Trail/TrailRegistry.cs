// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrailRegistry.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
                var oregonTrail = new[]
                {
                    new Location("Independence", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("Kansas River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Continental, null, RiverOption.FerryOperator), 
                    new Location("Big Blue River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Continental), 
                    new Location("Fort Kearney", 
                        LocationCategory.Settlement, Climate.Continental), 
                    new Location("Chimney Rock", 
                        LocationCategory.Landmark, Climate.Moderate), 
                    new Location("Fort Laramie", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("Independence Rock", 
                        LocationCategory.Landmark, Climate.Moderate), 
                    new Location("South Pass", 
                        LocationCategory.ForkInRoad, Climate.Dry, new List<Location>
                        {
                            new Location("Fort Bridger", LocationCategory.Settlement, Climate.Dry), 
                            new Location("Green River Shortcut", LocationCategory.Landmark, Climate.Dry)
                        }), 
                    new Location("Green River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Dry), 
                    new Location("Soda Springs", 
                        LocationCategory.Landmark, Climate.Dry), 
                    new Location("Fort Hall", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("Snake River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Moderate, null, RiverOption.IndianGuide), 
                    new Location("Fort Boise", 
                        LocationCategory.Settlement, Climate.Polar), 
                    new Location("Blue Mountains", 
                        LocationCategory.ForkInRoad, Climate.Polar, new List<Location>
                        {
                            new Location("Fort Walla Walla", LocationCategory.Settlement, Climate.Polar), 
                            new Location("The Dalles", LocationCategory.Landmark, Climate.Polar)
                        }), 
                    new Location("Oregon City", 
                        LocationCategory.Settlement, Climate.Moderate)
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
                var testTrail = new[]
                {
                    new Location("Start Settlement", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("Landmark", 
                        LocationCategory.Landmark, Climate.Dry), 
                    new Location("Fork In Road", 
                        LocationCategory.ForkInRoad, Climate.Continental, new List<Location>
                        {
                            new Location("Inserted Settlement", 
                                LocationCategory.Settlement, Climate.Polar), 
                            new Location("Inserted Landmark", 
                                LocationCategory.Landmark, Climate.Tropical)
                        }), 
                    new Location("River Crossing (with ferry)", 
                        LocationCategory.RiverCrossing, Climate.Continental, null, RiverOption.FerryOperator), 
                    new Location("River Crossing (with Indian)", 
                        LocationCategory.RiverCrossing, Climate.Continental, null, RiverOption.IndianGuide), 
                    new Location("River Crossing (default)", 
                        LocationCategory.RiverCrossing, Climate.Continental), 
                    new Location("End Settlement", 
                        LocationCategory.Settlement, Climate.Moderate)
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
                var testPoints = new[]
                {
                    new Location("Start Of Test", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("End Of Test", 
                        LocationCategory.Settlement, Climate.Dry)
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
                var testFail = new[]
                {
                    new Location("Start Of Test", 
                        LocationCategory.Settlement, Climate.Moderate), 
                    new Location("Wolf River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Continental, null, 
                        RiverOption.IndianGuide), 
                    new Location("Fox River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Moderate, null, 
                        RiverOption.IndianGuide), 
                    new Location("Otter River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Tropical, null, 
                        RiverOption.FerryOperator), 
                    new Location("Coyote River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Polar), 
                    new Location("Deer River Crossing", 
                        LocationCategory.RiverCrossing, Climate.Continental), 
                    new Location("End Of Test", 
                        LocationCategory.Settlement, Climate.Dry)
                };

                return new Trail(testFail, 50, 100);
            }
        }
    }
}