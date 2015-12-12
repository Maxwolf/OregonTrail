using System.Text;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     When crossing a river there is a chance that your wagon will flood if you choose to caulk and float across the
    ///     river.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross, false)]
    public sealed class VehicleFloods : EventItemDestroyer
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public VehicleFloods(EventCategory category) : base(category)
        {
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab.
        /// </summary>
        /// <returns>Returns a string that will be displayed to the user after event executes.</returns>
        protected override string OnEventPrompt()
        {
            var _eventText = new StringBuilder();
            _eventText.AppendLine("Vehicle floods ");
            _eventText.AppendLine("while crossing the ");
            _eventText.AppendLine("river results in ");
            _eventText.AppendLine("the loss of:");

            return _eventText.ToString();
        }
    }
}