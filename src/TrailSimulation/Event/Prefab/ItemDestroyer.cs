// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Prefab class that is used to destroy some items at random from the vehicle inventory. Will return a list of items
    ///     and print them to the screen and allow for a custom prompt message to be displayed so it can be different for each
    ///     implementation that wants to use it.
    /// </summary>
    public abstract class ItemDestroyer : EventProduct
    {
        /// <summary>
        ///     String builder that will hold all the data from event execution.
        /// </summary>
        private StringBuilder _eventText;

        /// <summary>
        ///     Fired when the event is created by the event factory, but before it is executed. Acts as a constructor mostly but
        ///     used in this way so that only the factory will call the method and there is no worry of it accidentally getting
        ///     called by creation.
        /// </summary>
        public override void OnEventCreate()
        {
            base.OnEventCreate();

            // Create the string builder that will hold representation of event action to display for debugging.
            _eventText = new StringBuilder();
        }

        /// <summary>
        ///     Rolls the dice and attempts to kill the passengers of the vehicle. If that happens then the killing verb will be
        ///     applied next to their name.
        /// </summary>
        /// <param name="killVerb">Action verb that describes how the person died such as burned, frozen, drowned, etc.</param>
        /// <returns>Formatted string that can be displayed on render for event item destruction.</returns>
        internal static string TryKillPassengers(string killVerb)
        {
            // Change event text depending on if items were destroyed or not.
            var postDestroy = new StringBuilder();
            postDestroy.AppendLine($"the loss of:{Environment.NewLine}");

            // Attempts to kill the living passengers of the vehicle.
            var drownedPassengers = GameSimulationApp.Instance.Vehicle.Passengers.TryKill();

            // If the killed passenger list contains any entries we print them out.
            var passengers = drownedPassengers as IList<Person> ?? drownedPassengers.ToList();
            foreach (var person in passengers)
            {
                // Only proceed if person is actually dead.
                if (person.HealthStatus != HealthStatus.Dead)
                    continue;

                // Last person killed will not add a new line.
                if (passengers.Last() == person)
                    postDestroy.Append($"{person.Name} ({killVerb})");
                else
                    postDestroy.AppendLine($"{person.Name} ({killVerb})");
            }

            // Returns the processed flooding event for rendering.
            return postDestroy.ToString();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo userData)
        {
            // Clear out the text from the string builder.
            _eventText.Clear();

            // Show the pre item destruction text if it exists.
            var preDestoyPrompt = OnPreDestroyItems();
            if (!string.IsNullOrEmpty(preDestoyPrompt))
                _eventText.AppendLine(preDestoyPrompt);

            // Destroy some items at random and get a list back of what and how much.
            var _destroyedItems = GameSimulationApp.Instance.Vehicle.DestroyRandomItems();

            // Show the post item destruction text if it exists.
            var postDestroyPrompt = OnPostDestroyItems(_destroyedItems);
            if (!string.IsNullOrEmpty(postDestroyPrompt))
                _eventText.AppendLine(postDestroyPrompt);

            // Skip if destroyed items count is zero.
            if (!(_destroyedItems?.Count > 0))
                return;

            // Loop through all of the destroyed items and add them to string builder.
            foreach (var destroyedItem in _destroyedItems)
            {
                // Last destroyed item will not append line.
                if (_destroyedItems.Last().Equals(destroyedItem))
                {
                    _eventText.Append($"{destroyedItem.Value.ToString("N0")} {destroyedItem.Key}");
                }
                else
                {
                    _eventText.AppendLine($"{destroyedItem.Value.ToString("N0")} {destroyedItem.Key}");
                }
            }
        }

        /// <summary>Fired by the item destroyer event prefab before items are destroyed.</summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        /// <returns>The <see cref="string" />.</returns>
        protected abstract string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems);

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected abstract string OnPreDestroyItems();

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return _eventText.ToString();
        }
    }
}