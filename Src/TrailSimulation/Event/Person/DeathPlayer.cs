using System;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Party leader has died! This will end the entire simulation since the others cannot go on without the leader.
    /// </summary>
    [DirectorEvent(EventCategory.Person, false)]
    public sealed class DeathPlayer : EventProduct
    {
        private StringBuilder _leaderDeath;

        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public DeathPlayer(EventCategory category) : base(category)
        {
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
        ///     Fired when the event is closed by the user or system after being executed and rendered out on text user interface.
        /// </summary>
        public override void OnEventClose()
        {
            base.OnEventClose();

            // Forcefully ends the game.
            GameSimulationApp.Instance.ShouldEndGame = true;
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender()
        {
            return _leaderDeath.ToString();
        }
    }
}