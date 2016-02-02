// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    /// <summary>
    ///     Event prefab that is intended to be used to indicate physical harm has come to a person in the vehicle. As such
    ///     this separates it from the infection flag since being injured means your physical capacity for work has been
    ///     diminished greatly. An example of this type of event would be the player breaking their arm.
    /// </summary>
    public abstract class PersonInjure : EventProduct
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
            var person = eventExecutor.SourceEntity as Person;

            // Sets flag on person making them more susceptible to further complications.
            person?.Injure();
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            // Cast the source entity as person.
            var person = userData.SourceEntity as Person;

            // Skip if the source entity is not a person.
            return person == null ? string.Empty : OnPostInjury(person);
        }

        /// <summary>Fired after the event has executed and the injury flag set on the person.</summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected abstract string OnPostInjury(Person person);
    }
}