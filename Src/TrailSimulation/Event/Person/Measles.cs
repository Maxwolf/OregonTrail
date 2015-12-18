using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Measles, also known as morbilli, rubeola or red measles, is a highly contagious infection caused by the measles
    ///     virus.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Measles : EventPersonInfect
    {
        /// <summary>
        ///     Fired after the event has executed and the infection flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has measles.";
        }
    }
}