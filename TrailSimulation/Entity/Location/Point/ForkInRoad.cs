// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Entity.Location.Point
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Weather;

    /// <summary>
    ///     Offers the vehicle multiple different choices about where it would like to split off and begin traveling to.
    ///     Depending on the choice the player makes the selected skip choice will be inserted into the trail as the next
    ///     location. After this happens the trail module will automatically re-calculate all the total distances and other
    ///     needed variables to make sure the map stays in sync.
    /// </summary>
    public sealed class ForkInRoad : Location
    {
        /// <summary>
        ///     List of possible alternate locations the player might have to choose from if this list is not null and greater than
        ///     one. Game simulation will ask using modes and states what choice player would like.
        /// </summary>
        private List<Location> skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForkInRoad" /> class. Initializes a new instance of the
        ///     <see cref="T:TrailSimulation.Entity.Location.Location" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="climateType">The climate Type.</param>
        /// <param name="skipChoices">The skip Choices.</param>
        public ForkInRoad(string name, Climate climateType, IEnumerable<Location> skipChoices) : base(name, climateType)
        {
            // Offers up a decision when traveling on the trail, there are normally one of many possible outcomes.
            if (skipChoices != null)
                this.skipChoices = new List<Location>(skipChoices);
        }

        /// <summary>
        ///     Defines all of the skip choices that were defined for this location. Will return null if there are no skip choices
        ///     associated with this location.
        /// </summary>
        public ReadOnlyCollection<Location> SkipChoices
        {
            get { return skipChoices.AsReadOnly(); }
        }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public override bool ChattingAllowed
        {
            get { return false; }
        }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public override bool ShoppingAllowed
        {
            get { return false; }
        }
    }
}