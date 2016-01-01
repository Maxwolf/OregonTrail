// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@5:56 AM

namespace TrailSimulation.Event.Weather
{
    using Entity.Vehicle;
    using Module.Director;
    using Window.RandomEvent;

    /// <summary>
    ///     Reduces the total capacity for the vehicle to move in a given trip segment by a random amount calculated at the
    ///     time of event execution.
    /// </summary>
    [DirectorEvent(EventCategory.Weather)]
    public sealed class HeavyFog : EventProduct
    {
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
            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle?.ReduceMileage(vehicle.Mileage - 10 - 5*GameSimulationApp.Instance.Random.Next());
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "lose your way in heavy fog---time is lost";
        }
    }
}