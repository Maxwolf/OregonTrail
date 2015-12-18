using System.Collections.Generic;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Prefab class that is used to destroy some items at random from the vehicle inventory. Will return a list of items
    ///     and print them to the screen and allow for a custom prompt message to be displayed so it can be different for each
    ///     implementation that wants to use it.
    /// </summary>
    public abstract class EventItemDestroyer : EventProduct
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
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(IEntity sourceEntity)
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
                _eventText.AppendLine($"{destroyedItem.Value.ToString("N0")} {destroyedItem.Key}");
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        protected abstract string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems);

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        protected abstract string OnPreDestroyItems();

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(IEntity sourceEntity)
        {
            return _eventText.ToString();
        }
    }
}