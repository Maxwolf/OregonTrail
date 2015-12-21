// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Snakebite.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Processes an attack of snake biting one of the passengers in the vehicle at random. Depending on the outcome of the
//   event we might kill the player if they actually get bit, otherwise the event will say they killed it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
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
        /// Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="userData">
        /// Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo userData)
        {
            // Cast the source entity as person.
            var person = userData.SourceEntity as Person;
            Debug.Assert(person != null, "person != null");

            // Ammo used to kill the snake.
            GameSimulationApp.Instance.Vehicle.Inventory[Entities.Ammo].ReduceQuantity(10);

            // Damage the person that was bit by the snake, it might be a little or a huge poisonousness bite.
            person.Damage(GameSimulationApp.Instance.Random.NextBool() ? 5 : 256);
        }

        /// <summary>
        /// Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        /// </param>
        /// <returns>
        /// Text user interface string that can be used to explain what the event did when executed.
        /// </returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "you killed a poisonous snake after it bit you";
        }
    }
}