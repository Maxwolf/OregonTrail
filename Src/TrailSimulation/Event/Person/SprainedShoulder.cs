using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     If you have sprained your acromioclavicular joint (the joint at the top of your shoulder), you may be advised to
    ///     avoid activities that involve moving your arm.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class SprainedShoulder : EventPersonInjure
    {
        /// <summary>
        ///     Fired after the event has executed and the injury flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} has a sprained shoulder.";
        }
    }
}