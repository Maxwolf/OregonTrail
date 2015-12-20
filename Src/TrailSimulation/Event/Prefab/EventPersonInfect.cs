using System.Diagnostics;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Intended to be used to make adding the infected flag to people easier. If an event wants to act as some sort of
    ///     biological agent then it can use this prefab and just worry about the message it prints and the action of infecting
    ///     the player will be done by this class.
    /// </summary>
    public abstract class EventPersonInfect : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventInfo">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo eventInfo)
        {
            // Cast the source entity as person.
            var person = eventInfo.SourceEntity as Person;
            Debug.Assert(person != null, "person != null");

            // Sets flag on person making them more susceptible to further complications.
            person.Infect();
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo eventInfo)
        {
            // Cast the source entity as person.
            var person = eventInfo.SourceEntity as Person;
            Debug.Assert(person != null, "person != null");

            return OnPostInfection(person);
        }

        /// <summary>
        ///     Fired after the event has executed and the infection flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected abstract string OnPostInfection(Person person);
    }
}