// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SufferingExhaustion.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   A person with weakness or fatigue lacks energy, feels weary, and is constantly tired.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Event
{
    using System.Diagnostics.CodeAnalysis;
    using Entity;
    using Game;

    /// <summary>
    ///     A person with weakness or fatigue lacks energy, feels weary, and is constantly tired.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class SufferingExhaustion : EventPersonInjure
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