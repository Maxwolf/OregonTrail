// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SprainedMuscle.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   The most common soft tissues injured are muscles, tendons, and ligaments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Event
{
    using System.Diagnostics.CodeAnalysis;
    using Entity;
    using Game;

    /// <summary>
    ///     The most common soft tissues injured are muscles, tendons, and ligaments.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class SprainedMuscle : EventPersonInjure
    {
        /// <summary>Fired after the event has executed and the injury flag set on the person.</summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} has sprained a muscle.";
        }
    }
}