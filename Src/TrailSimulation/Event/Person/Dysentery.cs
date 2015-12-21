// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dysentery.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Infection of the intestines resulting in severe diarrhea with the presence of blood and mucus in the feces.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Infection of the intestines resulting in severe diarrhea with the presence of blood and mucus in the feces.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Dysentery : EventPersonInfect
    {
        /// <summary>
        /// Fired after the event has executed and the infection flag set on the person.
        /// </summary>
        /// <param name="person">
        /// Person whom is now infected by whatever you say they are here.
        /// </param>
        /// <returns>
        /// Name or type of infection the person is currently affected with.
        /// </returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has dysentery.";
        }
    }
}