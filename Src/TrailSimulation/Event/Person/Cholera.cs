using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Cholera is an infection of the small intestine by some strains of the bacterium Vibrio cholerae. Symptoms may range
    ///     from none, to mild, to severe.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    public sealed class Cholera : EventPersonInfect
    {
        /// <summary>
        ///     Fired after the event has executed and the infection flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has cholera.";
        }
    }
}