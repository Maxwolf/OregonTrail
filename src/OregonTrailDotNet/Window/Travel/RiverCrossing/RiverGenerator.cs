// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Entity.Location.Point;

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

            // A river is not a dice roll: it is however deep this particular river runs, plus everything that has rained
            // on the country lately. That is the whole reason a summer crossing is a paddle and a spring one drowns
            // people - the Kansas is a foot deep in its bed, but April snowmelt puts nine feet of water over it.
            var wetness = game.Climate?.Wetness ?? 0;
            RiverDepth = Math.Round(riverLocation.BaseDepth + 2*wetness, 1);
            RiverWidth = (int) (riverLocation.BaseWidth + 15*wetness);
            RiverSpeed = riverLocation.BaseSpeed + wetness;
            RiverBottom = riverLocation.Bottom;

            // Determines how the player will want to cross the river.
            CrossingType = RiverCrossChoiceEnum.None;

            // Only setup ferry cost and delay if this is that type of crossing.
            switch (riverLocation.RiverCrossOption)
            {
                case RiverOptionEnum.FerryOperator:
                    // The ferryman's price is fixed at five dollars; what varies is how backed up he is, anywhere from
                    // two to six days' worth of wagons ahead of yours.
                    IndianCost = 0;
                    FerryCost = 5;
                    FerryDelayInDays = game.Random.Next(2, 7);
                    break;
                case RiverOptionEnum.FloatAndFord:
                    IndianCost = 0;
                    FerryCost = 0;
                    FerryDelayInDays = 0;
                    break;
                case RiverOptionEnum.IndianGuide:
                    // The Shoshoni guide asks two or three sets of clothing to take the wagon across, and that is the whole
                    // of it - he does not haggle, and he does not care how much game you have shot.
                    IndianCost = game.Random.Next(2, 4);
                    FerryCost = 0;
                    FerryDelayInDays = 0;
                    break;
                case RiverOptionEnum.None:
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
        public RiverCrossChoiceEnum CrossingType { get; set; }

        /// <summary>
        ///     A ford deeper than this starts costing the party something.
        /// </summary>
        public const double SafeFordDepth = 2.5;

        /// <summary>
        ///     Past this depth a ford is genuinely dangerous rather than merely wet, and can drown people.
        /// </summary>
        public const double DangerousFordDepth = 3.0;

        /// <summary>
        ///     A river shallower than this cannot be floated - there is not enough water to carry a wagon.
        /// </summary>
        public const double FloatableDepth = 1.5;

        /// <summary>
        ///     Determines how deep the river is in feet.
        /// </summary>
        public double RiverDepth { get; }

        /// <summary>
        ///     How fast the water is moving. Speed rather than depth is what rolls a floating wagon.
        /// </summary>
        public double RiverSpeed { get; }

        /// <summary>
        ///     What the riverbed is like underfoot on a crossing shallow enough to be safe.
        /// </summary>
        public RiverBottomEnum RiverBottom { get; }

        /// <summary>
        ///     Whether the water is shallow enough that a wagon would ground rather than float.
        /// </summary>
        public bool TooShallowToFloat => RiverDepth < FloatableDepth;

        /// <summary>
        ///     Whether the water is too shallow for the ferry to run today.
        /// </summary>
        public bool TooShallowForFerry => RiverDepth < SafeFordDepth;

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