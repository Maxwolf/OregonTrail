// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Diagnostics.CodeAnalysis;
using OregonTrailDotNet.TrailSimulation.Event.Prefab;
using OregonTrailDotNet.TrailSimulation.Module.Director;

namespace OregonTrailDotNet.TrailSimulation.Event.Person
{
    /// <summary>
    ///     Fever, also known as pyrexia and febrile response, is defined as having a temperature above the normal range due to
    ///     an increase in the body's temperature.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Fever : PersonInfect
    {
        /// <summary>Fired after the event has executed and the infection flag set on the person.</summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Entity.Person.Person person)
        {
            return $"{person.Name} has dysentery.";
        }
    }
}