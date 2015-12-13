using System.Collections.Generic;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Discover a vehicle on the side of the road that might have some items inside of it that will be added to the
    ///     players inventory.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    public sealed class AbandonedVehicle : EventItemCreator
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public AbandonedVehicle(EventCategory category) : base(category)
        {
        }

        /// <summary>
        ///     Fired by the event prefab after the event has executed.
        /// </summary>
        /// <param name="createdItems"></param>
        protected override string OnPostCreateItems(IDictionary<Entities, int> createdItems)
        {
            return createdItems.Count > 0 ? "and find:" : "but it is empty";
        }

        /// <summary>
        ///     Fired by the event prefab before the event has executed.
        /// </summary>
        protected override string OnPreCreateItems()
        {
            var _eventText = new StringBuilder();
            _eventText.AppendLine("You find an abandoned wagon,");
            return _eventText.ToString();
        }
    }
}