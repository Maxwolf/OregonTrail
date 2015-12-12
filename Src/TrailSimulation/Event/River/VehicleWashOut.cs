using System.Text;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Player forded the river and it was to deep, they have been washed out by the current and some items destroyed.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross, false)]
    public sealed class VehicleWashOut : EventItemDestroyer
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public VehicleWashOut(EventCategory category) : base(category)
        {
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab.
        /// </summary>
        /// <returns>Returns a string that will be displayed to the user after event executes.</returns>
        protected override string OnEventPrompt()
        {
            var _eventText = new StringBuilder();
            _eventText.AppendLine("Vehicle was washed ");
            _eventText.AppendLine("out when attempting to ");
            _eventText.AppendLine("ford the river results ");
            _eventText.AppendLine("in the loss of:");

            return _eventText.ToString();
        }
    }
}