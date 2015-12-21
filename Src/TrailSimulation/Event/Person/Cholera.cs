// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cholera.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Cholera is an infection of the small intestine by some strains of the bacterium Vibrio cholerae. Symptoms may range
//   from none, to mild, to severe.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Cholera is an infection of the small intestine by some strains of the bacterium Vibrio cholerae. Symptoms may range
    ///     from none, to mild, to severe.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Cholera : EventPersonInfect
    {
        /// <summary>Fired after the event has executed and the infection flag set on the person.</summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has cholera.";
        }
    }
}