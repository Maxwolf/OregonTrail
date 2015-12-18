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
    public abstract class EventItemCreator : EventProduct
    {
        /// <summary>
        ///     String builder that will hold all the data from event execution.
        /// </summary>
        private StringBuilder _eventText;

        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        protected EventItemCreator(EventCategory category)
        {
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

            // Add the pre-create message if it exists.
            var preCreatePrompt = OnPreCreateItems();
            if (!string.IsNullOrEmpty(preCreatePrompt))
                _eventText.AppendLine(preCreatePrompt);

            // Create some items at random and get a list back of what and how much.
            var createdItems = GameSimulationApp.Instance.Vehicle.CreateRandomItems();

            // Add the post create message if it exists.
            var postCreatePrompt = OnPostCreateItems(createdItems);
            if (!string.IsNullOrEmpty(postCreatePrompt))
                _eventText.AppendLine(postCreatePrompt);

            // Skip if created items count is zero.
            if (!(createdItems?.Count > 0))
                return;

            // Loop through all of the created items and add them to string builder.
            foreach (var createdItem in createdItems)
                _eventText.AppendLine($"{createdItem.Value.ToString("N0")} {createdItem.Key}");
        }

        /// <summary>
        ///     Fired by the event prefab after the event has executed.
        /// </summary>
        /// <param name="createdItems">Items that were created and added to vehicle inventory.</param>
        protected abstract string OnPostCreateItems(IDictionary<Entities, int> createdItems);

        /// <summary>
        ///     Fired by the event prefab before the event has executed.
        /// </summary>
        protected abstract string OnPreCreateItems();

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