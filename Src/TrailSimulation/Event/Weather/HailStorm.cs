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
    [DirectorEvent(EventCategory.Weather)]
    public sealed class HailStorm : EventItemDestroyer
    {
        /// <summary>
        ///     Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems"></param>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Grab an instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Change event text depending on if items were destroyed or not.
            var postDestroy = new StringBuilder();
            if (destroyedItems.Count > 0)
            {
                // Lists out the people and items destroyed.
                postDestroy.AppendLine("the loss of:");

                // Check if there are enough clothes to keep people warm, need two sets of clothes for every person.
                if (game.Vehicle.Inventory[Entities.Clothes].Quantity >= (game.Vehicle.PassengerLivingCount*2))
                    return postDestroy.ToString();

                // Attempts to kill the living passengers of the vehicle.
                var frozenPassengers = game.Vehicle.Passengers.TryKill();

                // If the killed passenger list contains any entries we print them out.
                foreach (var person in frozenPassengers)
                {
                    // Only proceed if person is actually dead.
                    if (person.Status == HealthLevel.Dead)
                        postDestroy.AppendLine($"{person.Name} (frozen)");
                }
            }
            else
            {
                // Got lucky nothing was destroyed!
                postDestroy.AppendLine("no loss of items.");
            }

            // Returns the processed text to event for rendering.
            return postDestroy.ToString();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(IEntity sourceEntity)
        {
            base.Execute(sourceEntity);

            // Cast the source entity as vehicle.
            var vehicle = sourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle.ReduceMileage(vehicle.Mileage - 5 - GameSimulationApp.Instance.Random.Next()*10);
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
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