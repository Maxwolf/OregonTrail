// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     Special data class that is used to generate river data in the travel info as requested. Creation of this object
    ///     will automatically create a new river with random width and depth.
    /// </summary>
    public sealed class RiverGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrailDotNet.Window.Travel.RiverCrossing.RiverGenerator" />
        ///     class.
        /// </summary>
        public RiverGenerator()
        {
            // Grab instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Cast the current location as river crossing.
            var riverLocation = game.Trail.CurrentLocation as Entity.Location.Point.RiverCrossing;
            if (riverLocation == null)
                throw new InvalidCastException(
                    "Unable to cast location as river crossing even though it returns as one!");

            // Randomly generates statistics about the river each time you cross it.
            RiverDepth = game.Random.Next(1, 20);

            // Determines how long the player will spend crossing river.
            RiverWidth = game.Random.Next(100, 1500);

            // Determines how the player will want to cross the river.
            CrossingType = RiverCrossChoice.None;

            // Only setup ferry cost and delay if this is that type of crossing.
            switch (riverLocation.RiverCrossOption)
            {
                case RiverOption.FerryOperator:
                    IndianCost = 0;
                    FerryCost = game.Random.Next(3, 8);
                    FerryDelayInDays = game.Random.Next(1, 10);
                    break;
                case RiverOption.FloatAndFord:
                    IndianCost = 0;
                    FerryCost = 0;
                    FerryDelayInDays = 0;
                    break;
                case RiverOption.IndianGuide:
                    IndianCost = game.Random.Next(3, 8);
                    FerryCost = 0;
                    FerryDelayInDays = 0;
                    break;
                case RiverOption.None:
                    throw new ArgumentException(
                        "Unable to generate river without having options configured to some value other than NONE!");
                default:
                    throw new ArgumentException(
                        "Unable to figure out what the river option value should be! Check value being sent to river generator class!");
            }
        }

        /// <summary>
        ///     Determines if we have force triggered an event to destroy items in the vehicle.
        /// </summary>
        public bool DisasterHappened { get; set; }

        /// <summary>
        ///     Determines how the vehicle and party members would like to cross the river.
        /// </summary>
        public RiverCrossChoice CrossingType { get; set; }

        /// <summary>
        ///     Determines how deep the river is in feet.
        /// </summary>
        public int RiverDepth { get; }

        /// <summary>
        ///     Determines how much the ferry operator will charge to cross the river.
        /// </summary>
        public float FerryCost { get; set; }

        /// <summary>
        ///     Determines how many days of the simulation the ferry operator is backed up with other vehicles and cannot process
        ///     yours until this time.
        /// </summary>
        public int FerryDelayInDays { get; set; }

        /// <summary>
        ///     Determines how wide the river is and how big the dice roll is for something terrible happening to the vehicle and
        ///     party members.
        /// </summary>
        public int RiverWidth { get; }

        /// <summary>
        ///     Determines how many sets of clothes the Shoshoni Indian guide would like in exchange for taking vehicle across the
        ///     river.
        /// </summary>
        public int IndianCost { get; set; }
    }
}