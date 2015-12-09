using System;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Infects the source entity player that is casted. Will not change infections or overwrite then, or add multiple only
    ///     one infection at a time may be active on a person to keep the simulation simple.
    /// </summary>
    public sealed class InfectPlayer : EventProduct
    {
        /// <summary>
        ///     Holds all the data about what disease we infected the person with.
        /// </summary>
        private StringBuilder _infectionPrompt;

        /// <summary>
        ///     Person that will be infected with a randomly selected disease.
        /// </summary>
        private Person _susceptiblePerson;

        /// <summary>
        ///     Creates a new instance of an event product with the specified event type for reference purposes.
        /// </summary>
        /// <param name="category">
        ///     what type of event this will be, used for grouping and filtering and triggering events by type rather than type of.
        /// </param>
        public InfectPlayer(EventCategory category) : base(category)
        {
            _infectionPrompt = new StringBuilder();
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
            // Cast the source entity into a person.
            _susceptiblePerson = sourceEntity as Person;
            if (_susceptiblePerson == null)
                throw new InvalidCastException("Unable to cast event source entity into a person to give disease to!");

            // Infects the person with a random disease.
            _susceptiblePerson.Infect();

            // Build up message about the infection we can show to the player.
            _infectionPrompt.Clear();
            _infectionPrompt.AppendLine($"{_susceptiblePerson.Name} has");
            _infectionPrompt.Append($"{_susceptiblePerson.Infection}.");
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender()
        {
            return _infectionPrompt.ToString();
        }
    }
}