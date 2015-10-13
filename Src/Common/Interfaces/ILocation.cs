using System.Collections.ObjectModel;

namespace OregonTrail
{
    /// <summary>
    ///     Defines a place in the game where a vessel, party, and persons are allowed it exist at any given time. A location
    ///     will be set onto a party entity and it will be propagated to vessel, party, and every person in the party keeping
    ///     them all in sync.
    /// </summary>
    public interface ILocation : IEntity
    {
        Weather Weather { get; }
        ReadOnlyCollection<Party> Parties { get; }
        LocationFlag Description { get; }
        ReadOnlyCollection<RandomEventBase> Actions { get; }
        float ActionChance { get; }
    }
}