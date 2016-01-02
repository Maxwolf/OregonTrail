// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:36 AM

namespace TrailSimulation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     A person with weakness or fatigue lacks energy, feels weary, and is constantly tired.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class SufferingExhaustion : PersonInjure
    {
        /// <summary>Fired after the event has executed and the injury flag set on the person.</summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} is suffering from exhaustion.";
        }
    }
}