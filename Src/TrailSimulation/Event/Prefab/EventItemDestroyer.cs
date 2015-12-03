using System;
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
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        protected EventItemDestroyer(EventCategory category) : base(category)
        {
            // Create the string builder that will hold representation of event action to display for debugging.
            _eventText = new StringBuilder();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entity which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(IEntity sourceEntity)
        {
            // Clear out the text from the string builder.
            _eventText.Clear();

            // Fire override that will get us some item destroy prompt text.
            _eventText.AppendLine(OnEventPrompt());

            // Destroy some items at random and get a list back of what and how much.
            var _destroyedItems = GameSimulationApp.Instance.Vehicle.DestroyRandomItems();

            // Skip if the destroyed items is null.
            if (_destroyedItems == null)
                return;

            // Skip if destroyed items count is zero.
            if (_destroyedItems.Count > 0)
            {
                // Loop through all of the destroyed items and add them to string builder.
                foreach (var destroyedItem in _destroyedItems)
                {
                    _eventText.AppendLine($"{destroyedItem.Value.ToString("N0")} {destroyedItem.Key}");
                }
            }
            else
            {
                // No items were destroyed!
                _eventText.AppendLine("No items were destroyed!");
            }
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab.
        /// </summary>
        /// <returns>Returns a string that will be displayed to the user after event executes.</returns>
        protected abstract string OnEventPrompt();

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender()
        {
            return _eventText.ToString();
        }
    }
}