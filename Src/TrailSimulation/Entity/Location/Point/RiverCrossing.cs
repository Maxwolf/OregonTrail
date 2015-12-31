// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/22/2015@3:46 AM

namespace TrailSimulation
{
    /// <summary>
    ///     Defines a river that the vehicle must cross when it encounters it. There are several options that can be used that
    ///     are specific to a river and allow it to be configured to have different types of crossings such as with a ferry
    ///     operator, and Indian guide, or neither and only supporting fording into the river and caulking and floating across.
    /// </summary>
    public sealed class RiverCrossing : Location
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RiverCrossing" /> class. Initializes a new instance of the
        ///     <see cref="T:TrailSimulation.Location" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="climateType">The climate Type.</param>
        /// <param name="riverOption">The river Option.</param>
        public RiverCrossing(string name, Climate climateType, RiverOption riverOption = RiverOption.FloatAndFord)
            : base(name, climateType)
        {
            // Set the river option into the location itself.
            RiverCrossOption = riverOption;
        }

        /// <summary>
        ///     Defines the type of river crossing this location will be, this is in regards to the types of options presented when
        ///     the crossing comes up in the trail.
        /// </summary>
        public RiverOption RiverCrossOption { get; }

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