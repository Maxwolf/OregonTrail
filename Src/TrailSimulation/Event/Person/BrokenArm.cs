// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrokenArm.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   One of the members of the vehicle passenger manifest broke their arm somehow.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     One of the members of the vehicle passenger manifest broke their arm somehow.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class BrokenArm : EventPersonInjure
    {
        /// <summary>
        /// Fired after the event has executed and the injury flag set on the person.
        /// </summary>
        /// <param name="person">
        /// Person whom is now injured by whatever you say they are here.
        /// </param>
        /// <returns>
        /// Describes what type of physical injury has come to the person.
        /// </returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} has broken their arm.";
        }
    }
}