// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using OregonTrailDotNet.Entity.Location.Point;

namespace OregonTrailDotNet.Window.Travel.Toll
{
    /// <summary>
    ///     Generates a new toll amount and keeps track of the location to be inserted if the deal goes through with the
    ///     player. Otherwise this information will be destroyed.
    /// </summary>
    public sealed class TollGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrailDotNet.Window.Travel.Toll.TollGenerator" /> class.
        /// </summary>
        /// <param name="tollRoad">Location that is going to cost the player money in order to use the path to travel to it.</param>
        public TollGenerator(TollRoad tollRoad)
        {
            // The Barlow road is not priced at random: it is five dollars for the wagon plus fifty cents a head for the
            // oxen pulling it, so a party dragging a full team of twenty pays fifteen dollars where a lean one pays six.
            var oxen = GameSimulationApp.Instance.Vehicle.Inventory[Entity.EntitiesEnum.Animal].Quantity;
            Cost = (int) (5f + oxen*0.5f);
            Road = tollRoad;
        }

        /// <summary>
        ///     Location that is going to cost the player money in order to use the path to travel to it.
        /// </summary>
        public TollRoad Road { get; }

        /// <summary>
        ///     Gets the total toll for the cost road the player must pay before they will be allowed on the cost road.
        /// </summary>
        public int Cost { get; }
    }
}