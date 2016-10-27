// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Diagnostics.CodeAnalysis;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Window.RandomEvent;

namespace OregonTrailDotNet.Event.Animal
{
    /// <summary>
    ///     Processes an attack of snake biting one of the passengers in the vehicle at random. Depending on the outcome of the
    ///     event we might kill the player if they actually get bit, otherwise the event will say they killed it.
    /// </summary>
    [DirectorEvent(EventCategory.Animal)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Snakebite : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventExecutor">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo eventExecutor)
        {
            // Cast the source entity as person.
            var person = eventExecutor.SourceEntity as Entity.Person.Person;

            // Skip if the source entity is not a person.
            if (person == null)
                return;

            // Ammo used to kill the snake.
            GameSimulationApp.Instance.Vehicle.Inventory[Entities.Ammo].ReduceQuantity(10);

            // Damage the person that was bit by the snake, it might be a little or a huge poisonousness bite.
            if (GameSimulationApp.Instance.Random.NextBool())
            {
                person.Infect();
                person.Damage(256);
            }
            else
            {
                person.Damage(5);
            }
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "You killed a poisonous snake, after it bit you.";
        }
    }
}