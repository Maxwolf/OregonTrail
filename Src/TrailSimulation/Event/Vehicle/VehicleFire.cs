using System.Diagnostics;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Fire in the vehicle occurs, there is a chance that some of the inventory items or people were burned to death.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    public sealed class VehicleFire : EventProduct
    {
        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public VehicleFire(EventCategory category) : base(category)
        {
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
            // Cast the source entity as vehicle.
            var vehicle = sourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Remove food, and ammo.
            vehicle.Inventory[Entities.Food].ReduceQuantity(40);
            vehicle.Inventory[Entities.Ammo].ReduceQuantity(400);

            // Damage the passengers randomly up to certain amount.
            vehicle.Passengers.Damage(GameSimulationApp.Instance.Random.Next()*68 - 3);

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle.ReduceMileage(15);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(IEntity sourceEntity)
        {
            return "there was a fire in your wagon--food and supplies damage!";
        }
    }
}