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
                // Distances are the 1985 original's own route table, recovered from VAR.BIN (see
                // legacy/source/VAR.BIN.txt). A location's TotalDistance is the leg leading away from it; a fork branch
                // additionally carries LegDistance, the road from the fork to the branch. The standard route runs 1,871
                // miles, which is the "2000 miles" the game's own opening text promises.
                var oregonTrail = new Location[]
                {
                    // The five legs out to Fort Laramie run across open plains at twenty miles a day; everything west of it
                    // is mountain country at twelve, which is the default every other location keeps.
                    new Settlement("Independence", ClimateEnum.MissouriValley)
                        { TotalDistance = 102, BaseMilesPerDay = Location.PlainsMilesPerDay },
                    new RiverCrossing("Kansas River Crossing", ClimateEnum.MissouriValley, RiverOptionEnum.FerryOperator)
                    {
                        TotalDistance = 83, BaseMilesPerDay = Location.PlainsMilesPerDay,
                        BaseDepth = 1.0, BaseWidth = 600, BaseSpeed = 3.0, Bottom = RiverBottomEnum.Firm
                    },
                    new RiverCrossing("Big Blue River Crossing", ClimateEnum.MissouriValley)
                    {
                        TotalDistance = 119, BaseMilesPerDay = Location.PlainsMilesPerDay,
                        BaseDepth = 1.0, BaseWidth = 220, BaseSpeed = 2.0, Bottom = RiverBottomEnum.Muddy
                    },
                    new Settlement("Fort Kearney", ClimateEnum.GreatPlains)
                        { TotalDistance = 250, BaseMilesPerDay = Location.PlainsMilesPerDay },
                    new Landmark("Chimney Rock", ClimateEnum.GreatPlains)
                        { TotalDistance = 86, BaseMilesPerDay = Location.PlainsMilesPerDay },
                    new Settlement("Fort Laramie", ClimateEnum.GreatPlains) { TotalDistance = 190 },
                    new Landmark("Independence Rock", ClimateEnum.HighCountry) { TotalDistance = 102 },

                    // Taking the Fort Bridger road means one less river to cross: the Green River crossing is the other
                    // branch of this same fork, not a location on the main trail, so choosing the fort skips it outright.
                    // It costs 86 miles to do so - 125 + 162 by way of the fort against 57 + 144 through the river.
                    // The fork's own distance is never used, since neither branch is the main trail.
                    new ForkInRoad("South Pass", ClimateEnum.HighCountry, new List<Location>
                    {
                        new Settlement("Fort Bridger", ClimateEnum.HighCountry) { LegDistance = 125, TotalDistance = 162 },
                        new RiverCrossing("Green River Crossing", ClimateEnum.HighCountry, RiverOptionEnum.FerryOperator)
                        {
                            LegDistance = 57, TotalDistance = 144,
                            // Twenty feet deep in its own bed: the reason the Fort Bridger road exists.
                            BaseDepth = 20.0, BaseWidth = 400, BaseSpeed = 5.0, Bottom = RiverBottomEnum.Rough
                        }
                    }) { HighGround = true, StuckChance = 80, TotalDistance = 57 },

                    new Landmark("Soda Springs", ClimateEnum.HighCountry) { TotalDistance = 57 },
                    new Settlement("Fort Hall", ClimateEnum.SnakeRiverPlain) { TotalDistance = 182 },
                    new RiverCrossing("Snake River Crossing", ClimateEnum.SnakeRiverPlain, RiverOptionEnum.IndianGuide)
                    {
                        TotalDistance = 114,
                        BaseDepth = 6.0, BaseWidth = 1000, BaseSpeed = 7.0, Bottom = RiverBottomEnum.Rough
                    },
                    new Settlement("Fort Boise", ClimateEnum.SnakeRiverPlain) { TotalDistance = 160 },

                    // Fort Walla Walla is a detour, not a parting of ways: both routes out of the Blue Mountains arrive at
                    // The Dalles, so the second choice is a null - stay on the main trail - rather than a branch. Going by
                    // way of the fort is 55 + 120 miles against 125 straight on, which is the fork's own distance.
                    new ForkInRoad("Blue Mountains", ClimateEnum.PacificSlope, new List<Location>
                    {
                        new Settlement("Fort Walla Walla", ClimateEnum.PacificSlope) { LegDistance = 55, TotalDistance = 120 },
                        null
                    }) { HighGround = true, StuckChance = 70, TotalDistance = 125 },

                    // The Dalles sits on the main trail and cannot be avoided, which is what makes its choice - run the
                    // Columbia for free, or pay the Barlow toll - the last real decision of the journey. Either way it is
                    // the same 100 miles to the valley.
                    new ForkInRoad("The Dalles", ClimateEnum.PacificSlope, new List<Location>
                    {
                        // The Columbia is not forded or floated in the original at all - it is run on a raft, which this port has no
                        // equivalent of - so it is given the deepest, fastest water on the trail and left at that.
                        new RiverCrossing("Columbia River", ClimateEnum.PacificSlope)
                        {
                            LocksPartyHealth = true, RaftCrossing = true, TotalDistance = 100,
                            BaseDepth = 20.0, BaseWidth = 600, BaseSpeed = 8.0, Bottom = RiverBottomEnum.Rough
                        },
                        new TollRoad("Barlow Toll Road", ClimateEnum.PacificSlope) { TotalDistance = 100 }
                    }) { TotalDistance = 100 },

                    new Settlement("Oregon City", ClimateEnum.PacificSlope)
                };

                return new Trail(oregonTrail);
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
                    new Settlement("Start Of Test", ClimateEnum.MissouriValley),
                    new Settlement("End Of Test", ClimateEnum.PacificSlope)
                };

                return new Trail(testPoints, 50, 100);
            }
        }
    }
}