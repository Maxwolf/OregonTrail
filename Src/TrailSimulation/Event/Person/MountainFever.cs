// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MountainFever.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Rocky Mountain spotted fever (RMSF), also known as blue disease, is the most lethal and most frequently reported
//   rickettsial illness in the United States.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Rocky Mountain spotted fever (RMSF), also known as blue disease, is the most lethal and most frequently reported
    ///     rickettsial illness in the United States.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class MountainFever : EventPersonInfect
    {
        /// <summary>Fired after the event has executed and the infection flag set on the person.</summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has mountain fever.";
        }
    }
}