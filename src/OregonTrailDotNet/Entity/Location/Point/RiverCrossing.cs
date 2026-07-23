// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Window.Travel.RiverCrossing;

namespace OregonTrailDotNet.Entity.Location.Point
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
        ///     <see cref="T:OregonTrailDotNet.Entity.Location.Location" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="climateType">The climate Type.</param>
        /// <param name="riverOption">The river Option.</param>
        public RiverCrossing(string name, ClimateEnum climateType, RiverOptionEnum riverOption = RiverOptionEnum.FloatAndFord)
            : base(name, climateType)
        {
            // Set the river option into the location itself.
            RiverCrossOption = riverOption;
        }

        /// <summary>
        ///     Defines the type of river crossing this location will be, this is in regards to the types of options presented when
        ///     the crossing comes up in the trail.
        /// </summary>
        public RiverOptionEnum RiverCrossOption { get; }

        /// <summary>
        ///     How deep this river runs in feet before any rain, from the original's own river table. What the party actually
        ///     finds is this plus twice however wet the country has become, which is why the same ford is a two-foot paddle in
        ///     high summer and nine feet of snowmelt in March.
        /// </summary>
        public double BaseDepth { get; set; } = 1.0;

        /// <summary>
        ///     How wide this river runs in feet before any rain. Rain widens it as well as deepening it.
        /// </summary>
        public int BaseWidth { get; set; } = 600;

        /// <summary>
        ///     How fast the water moves before any rain. Speed is what capsizes a floating wagon rather than depth.
        /// </summary>
        public double BaseSpeed { get; set; } = 3.0;

        /// <summary>
        ///     What the riverbed is like underfoot, which decides what can go wrong on an otherwise safe ford.
        /// </summary>
        public RiverBottomEnum Bottom { get; set; } = RiverBottomEnum.Firm;

        /// <summary>
        ///     Whether choosing this crossing freezes the party's health for the endgame tally. Only the Columbia run does:
        ///     committing to it is the last decision of the journey, and whatever health the party carries into the water is
        ///     the health they are scored on no matter what the river does to them afterwards.
        /// </summary>
        public bool LocksPartyHealth { get; set; }

        /// <summary>
        ///     Whether this crossing is run on a raft — the Columbia, and only the Columbia. With presentation on it
        ///     plays as the original's FLOAT minigame instead of the ford/float menu; headless hosts keep the
        ///     deep-water stand-in either way.
        /// </summary>
        public bool RaftCrossing { get; set; }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public override bool ChattingAllowed => false;

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public override bool ShoppingAllowed => false;
    }
}