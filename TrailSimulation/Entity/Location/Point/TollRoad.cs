// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Entity.Location.Point
{
    using Weather;

    /// <summary>
    ///     Defines a location on the trail where the player is required to pay monies in order to use it. Typically this is
    ///     inserted as a skip location for a fork in the road, however it could be used as a normal location on the trail but
    ///     if the player did not have enough money they would be unable to continue down the trail.
    /// </summary>
    public sealed class TollRoad : Location
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TollRoad" /> class. Initializes a new instance of the
        ///     <see cref="T:TrailSimulation.Entity.Location.Location" /> class.
        /// </summary>
        /// <param name="name">Display name of the location as it should be known to the player.</param>
        /// <param name="climateType">Defines the type of weather the location will have overall.</param>
        public TollRoad(string name, Climate climateType) : base(name, climateType)
        {
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