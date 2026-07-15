// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Window.Travel.RiverCrossing;

namespace OregonTrailDotNet.Module.Trail
{
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
                    new Settlement("Independence", ClimateEnum.Moderate),
                    new RiverCrossing("Kansas River Crossing", ClimateEnum.Continental, RiverOptionEnum.FerryOperator),
                    new RiverCrossing("Big Blue River Crossing", ClimateEnum.Continental),
                    new Settlement("Fort Kearney", ClimateEnum.Continental),
                    new Landmark("Chimney Rock", ClimateEnum.Moderate),
                    new Settlement("Fort Laramie", ClimateEnum.Moderate),
                    new Landmark("Independence Rock", ClimateEnum.Moderate),
                    new ForkInRoad("South Pass", ClimateEnum.Dry, new List<Location>
                    {
                        new Settlement("Fort Bridger", ClimateEnum.Dry),
                        new Landmark("Green River Shortcut", ClimateEnum.Dry)
                    }) { HighGround = true, StuckChance = 80 },
                    new RiverCrossing("Green River Crossing", ClimateEnum.Dry),
                    new Landmark("Soda Springs", ClimateEnum.Dry),
                    new Settlement("Fort Hall", ClimateEnum.Moderate),
                    new RiverCrossing("Snake River Crossing", ClimateEnum.Moderate, RiverOptionEnum.IndianGuide),
                    new Settlement("Fort Boise", ClimateEnum.Polar),
                    new ForkInRoad("Blue Mountains", ClimateEnum.Polar, new List<Location>
                    {
                        new Settlement("Fort Walla Walla", ClimateEnum.Polar),
                        new ForkInRoad("The Dalles", ClimateEnum.Polar, new List<Location>
                        {
                            new RiverCrossing("Columbia River", ClimateEnum.Moderate),
                            new TollRoad("Barlow Toll Road", ClimateEnum.Moderate)
                        })
                    }) { HighGround = true, StuckChance = 70 },
                    new Settlement("Oregon City", ClimateEnum.Moderate)
                };

                return new Trail(oregonTrail, 32, 164);
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
                    new Settlement("Start Of Test", ClimateEnum.Moderate),
                    new Settlement("End Of Test", ClimateEnum.Dry)
                };

                return new Trail(testPoints, 50, 100);
            }
        }
    }
}