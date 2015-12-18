using System;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Party leader has died! This will end the entire simulation since the others cannot go on without the leader.
    /// </summary>
    [DirectorEvent(EventCategory.Person, EventExecution.ManualOnly)]
    public sealed class DeathPlayer : EventProduct
    {
        private StringBuilder _leaderDeath;

        /// <summary>
        ///     Fired when the event is created by the event factory, but before it is executed. Acts as a constructor mostly but
        ///     used in this way so that only the factory will call the method and there is no worry of it accidentally getting
        ///     called by creation.
        /// </summary>
        public override void OnEventCreate()
        {
            base.OnEventCreate();

            _leaderDeath = new StringBuilder();
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
            // Cast the source entity as a player.
            var sourcePerson = sourceEntity as Person;
            if (sourcePerson == null)
                throw new ArgumentNullException(nameof(sourceEntity), "Could not cast source entity as player.");

            // Check to make sure this player is the leader (aka the player).
            if (!sourcePerson.IsLeader)
                throw new ArgumentException("Cannot kill this person because it is not the player!");

            _leaderDeath.AppendLine($"{sourcePerson.Name} has died.");
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(IEntity sourceEntity)
        {
            return _leaderDeath.ToString();
        }
    }
}