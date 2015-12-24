// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/22/2015@3:40 AM

namespace TrailSimulation.Entity
{
    using System.Collections.ObjectModel;

    /// <summary>
    ///     Base interface used for all locations on the trail, the purpose of which is to abstract the functionality of the
    ///     location so we can create many different concrete implementations and then cast to the appropriate type in game
    ///     window.
    /// </summary>
    public interface ILocation : IEntity
    {
        /// <summary>
        ///     Warnings about low food, medical problems, weather, etc.
        /// </summary>
        LocationWarning Warning { get; }

        /// <summary>
        ///     Current weather condition this location is experiencing.
        /// </summary>
        Weather Weather { get; }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        bool ChattingAllowed { get; }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        bool ShoppingAllowed { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        LocationStatus Status { get; set; }

        /// <summary>
        ///     References all of the possible trades this location will be able to offer the player. If the list is empty that
        ///     means nobody wants to trade with the player at this time.
        /// </summary>
        ReadOnlyCollection<SimItem> Trades { get; }

        /// <summary>
        ///     Determines if the look around question has been asked in regards to the player stopping the vehicle to rest or
        ///     change vehicle options. Otherwise they will just continue on the trail, this property prevents the question from
        ///     being asked twice for any one location.
        /// </summary>
        bool ArrivalFlag { get; set; }

        /// <summary>
        ///     Determines if the given location is the last location on the trail, this is useful to know because we want to do
        ///     something special with the location before we actually arrive to it but we can know the next location is last using
        ///     this.
        /// </summary>
        bool LastLocation { get; set; }

        /// <summary>
        ///     Total distance to the next location the player must travel before it will be triggered
        /// </summary>
        int TotalDistance { get; set; }
    }
}