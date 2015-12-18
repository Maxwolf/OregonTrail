using System.Diagnostics;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Indians help you find some free food, this event will be called manually more often if you are low on food to
    ///     simulate the effect of them noticing you need help.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    public sealed class IndianFindFood : EventProduct
    {
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
            // Cast the source entity as vehicle.
            var vehicle = sourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Indians hook you up with free food, what nice guys.
            vehicle.Inventory[Entities.Food].AddQuantity(14);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(IEntity sourceEntity)
        {
            return "helpful Indians show you where to find more food";
        }
    }
}