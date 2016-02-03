// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Measles, also known as morbilli, rubeola or red measles, is a highly contagious infection caused by the measles
    ///     virus.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class Measles : PersonInfect
    {
        /// <summary>Fired after the event has executed and the infection flag set on the person.</summary>
        /// <param name="person">Person whom is now infected by whatever you say they are here.</param>
        /// <returns>Name or type of infection the person is currently affected with.</returns>
        protected override string OnPostInfection(Person person)
        {
            return $"{person.Name} has measles.";
        }
    }
}