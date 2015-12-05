using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public sealed class Location
    {
        /// <summary>
        ///     List of possible alternate locations the player might have to choose from if this list is not null and greater than
        ///     one. Game simulation will ask using modes and states what choice player would like.
        /// </summary>
        private List<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.Location" /> class.
        /// </summary>
        public Location(string name, LocationCategory category, IEnumerable<Location> skipChoices = null)
        {
            // Offers up a decision when traveling on the trail, there are normally one of many possible outcomes.
            if (skipChoices != null)
                _skipChoices = new List<Location>(skipChoices);

            // Name of the point as it should be known to the player.
            Name = name;

            // Category of the location determines how the game simulation will treat it.
            Category = category;
            switch (category)
            {
                case LocationCategory.Landmark:
                    ShoppingAllowed = false;
                    RiverCrossing = false;
                    HuntingAllowed = true;
                    TradingAllowed = true;
                    ChattingAllowed = false;
                    break;
                case LocationCategory.Settlement:
                    ShoppingAllowed = true;
                    RiverCrossing = false;
                    HuntingAllowed = false;
                    TradingAllowed = true;
                    ChattingAllowed = true;
                    break;
                case LocationCategory.RiverCrossing:
                    ShoppingAllowed = false;
                    RiverCrossing = true;
                    HuntingAllowed = false;
                    TradingAllowed = false;
                    ChattingAllowed = false;
                    break;
                case LocationCategory.ForkInRoad:
                    ShoppingAllowed = false;
                    RiverCrossing = false;
                    HuntingAllowed = false;
                    TradingAllowed = false;
                    ChattingAllowed = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }

            // Default location status is not visited by the player or vehicle.
            Status = LocationStatus.Unreached;
        }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public bool ChattingAllowed { get; private set; }

        /// <summary>
        ///     Determines if this location allows for trading to occur.
        /// </summary>
        public bool TradingAllowed { get; private set; }

        /// <summary>
        ///     Determines if this location allows for hunting to occur.
        /// </summary>
        public bool HuntingAllowed { get; private set; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public static Mode Mode
        {
            get { return Mode.Travel; }
        }

        /// <summary>
        ///     Defines the type of location this is, the game simulation will trigger and load different states depending on this
        ///     value. Defaults to default value which is a normal location with nothing special happening.
        /// </summary>
        public LocationCategory Category { get; }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public bool ShoppingAllowed { get; }

        /// <summary>
        ///     Determines if this location is an obstruction to the player that needs to be crossed.
        /// </summary>
        public bool RiverCrossing { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public LocationStatus Status { get; set; }

        /// <summary>
        ///     Defines all of the skip choices that were defined for this location. Will return null if there are no skip choices
        ///     associated with this location.
        /// </summary>
        public ReadOnlyCollection<Location> SkipChoices
        {
            get { return _skipChoices.AsReadOnly(); }
        }
    }
}