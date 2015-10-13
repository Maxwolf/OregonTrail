using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Defines a place in the game where a vessel, party, and persons are allowed it exist at any given time. A location
    ///     will be set onto a party entity and it will be propagated to vessel, party, and every person in the party keeping
    ///     them all in sync.
    /// </summary>
    public interface ILocation : IEntity
    {
        WeatherTier Weather { get; }
        ReadOnlyCollection<IParty> Parties { get; }
        LocationCategory Description { get; }
        ReadOnlyCollection<IRandomEvent> Actions { get; }
        float ActionChance { get; }
    }
}