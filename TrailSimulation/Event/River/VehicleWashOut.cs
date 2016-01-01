// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:18 AM

namespace TrailSimulation.Event.River
{
    using System.Collections.Generic;
    using System.Text;
    using Entity;
    using Entity.Vehicle;
    using Module.Director;
    using Prefab;
    using Window.RandomEvent;

    /// <summary>
    ///     Player forded the river and it was to deep, they have been washed out by the current and some items destroyed.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross, EventExecution.ManualOnly)]
    public sealed class VehicleWashOut : ItemDestroyer
    {
        /// <summary>Fired by the item destroyer event prefab before items are destroyed.</summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        /// <returns>The <see cref="string" />.</returns>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            return destroyedItems.Count > 0
                ? TryKillPassengers("drowned")
                : "no loss of items.";
        }

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
            base.Execute(userData);

            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle?.ReduceMileage(20 - 20*GameSimulationApp.Instance.Random.Next());
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnPreDestroyItems()
        {
            var _eventText = new StringBuilder();
            _eventText.AppendLine("Vehicle was washed");
            _eventText.AppendLine("out when attempting to");
            _eventText.Append("ford the river results");
            return _eventText.ToString();
        }
    }
}