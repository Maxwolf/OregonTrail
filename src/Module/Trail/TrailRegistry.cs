// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
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
    }
}