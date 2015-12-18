using System.Collections.Generic;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Robber who can come in the middle of the night and steal things from the vehicle inventory. He is also very
    ///     dangerous and will do whatever it takes to get what he wants, so there is a chance some of your party members may
    ///     get murdered.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    public sealed class Thief : EventItemDestroyer
    {
        /// <summary>
        ///     Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
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
                var murderedPassengers = game.Vehicle.Passengers.TryKill();

                // If the killed passenger list contains any entries we print them out.
                foreach (var person in murderedPassengers)
                {
                    // Only proceed if person is actually dead.
                    if (person.Status == HealthLevel.Dead)
                        postDestroy.AppendLine($"{person.Name} (murdered)");
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
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        protected override string OnPreDestroyItems()
        {
            var theifPrompt = new StringBuilder();
            theifPrompt.Clear();
            theifPrompt.AppendLine("Thief comes in the");
            theifPrompt.Append("night resulting in ");
            return theifPrompt.ToString();
        }
    }
}