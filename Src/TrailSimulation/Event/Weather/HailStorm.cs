// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HailStorm.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Bad hail storm damages supplies, this uses the item destroyer prefab like the river crossings do.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Bad hail storm damages supplies, this uses the item destroyer prefab like the river crossings do.
    /// </summary>
    [DirectorEvent(EventCategory.Weather, EventExecution.ManualOnly)]
    public sealed class HailStorm : EventItemDestroyer
    {
        /// <summary>Fired by the item destroyer event prefab before items are destroyed.</summary>
        /// <param name="destroyedItems"></param>
        /// <returns>The <see cref="string"/>.</returns>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Grab an instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Check if there are enough clothes to keep people warm, need two sets of clothes for every person.
            return game.Vehicle.Inventory[Entities.Clothes].Quantity >= (game.Vehicle.PassengerLivingCount*2) &&
                   destroyedItems.Count < 0
                ? "no loss of items."
                : TryKillPassengers("frozen");
        }

        /// <summary>Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.</summary>
        /// <param name="userData">Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.</param>
        public override void Execute(RandomEventInfo userData)
        {
            base.Execute(userData);

            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle.ReduceMileage(vehicle.Mileage - 5 - GameSimulationApp.Instance.Random.Next()*10);
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnPreDestroyItems()
        {
            var _floodPrompt = new StringBuilder();
            _floodPrompt.Clear();
            _floodPrompt.AppendLine("Severe hail storm");
            _floodPrompt.Append("results in");
            return _floodPrompt.ToString();
        }
    }
}