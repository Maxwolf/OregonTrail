// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:36 AM

namespace TrailSimulation.Event.Person
{
    using System.Diagnostics.CodeAnalysis;
    using Entity.Person;
    using Module.Director;
    using Prefab;

    /// <summary>
    ///     Rocky Mountain spotted fever (RMSF), also known as blue disease, is the most lethal and most frequently reported
    ///     rickettsial illness in the United States.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class MountainFever : PersonInfect
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