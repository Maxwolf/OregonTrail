// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Concussion.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Temporary unconsciousness caused by a blow to the head. The term is also used loosely of the aftereffects such as
//   confusion or temporary incapacity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Event
{
    using System.Diagnostics.CodeAnalysis;
    using Entity;
    using Game;

    /// <summary>
    ///     Temporary unconsciousness caused by a blow to the head. The term is also used loosely of the aftereffects such as
    ///     confusion or temporary incapacity.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Concussion : EventPersonInjure
    {
        /// <summary>Fired after the event has executed and the injury flag set on the person.</summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} has a concussion.";
        }
    }
}