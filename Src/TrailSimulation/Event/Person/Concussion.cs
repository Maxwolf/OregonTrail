using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Temporary unconsciousness caused by a blow to the head. The term is also used loosely of the aftereffects such as
    ///     confusion or temporary incapacity.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    public sealed class Concussion : EventPersonInfect
    {
        /// <summary>
        ///     Fired after the event has executed and the infection flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has a concussion.";
        }
    }
}