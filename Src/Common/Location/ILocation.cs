using System.Collections.ObjectModel;
using OregonTrail.Entity;

namespace OregonTrail.Common
{
    /// <summary>
    ///     Defines a place in the game where a vessel, party, and persons are allowed it exist at any given time. A location
    ///     will be set onto a party entity and it will be propagated to vessel, party, and every person in the party keeping
    ///     them all in sync.
    /// </summary>
    public interface ILocation : IEntity
    {
        WeatherTier Weather { get; }
        ReadOnlyCollection<Party> Parties { get; }
        LocationCategory Description { get; }
        ReadOnlyCollection<RandomEventBase> Actions { get; }
        float ActionChance { get; }
    }
}