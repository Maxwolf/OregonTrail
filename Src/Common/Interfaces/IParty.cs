using System.Collections.ObjectModel;

namespace OregonTrail
{
    /// <summary>
    ///     Party is a defined list of persons entities that make up the party member property. Vessel is a entity that all the
    ///     party travels in and has it's own health and parts that make it operate.
    /// </summary>
    public interface IParty : IEntity
    {
        ReadOnlyCollection<Person> Members { get; }
        Vehicle Vessel { get; }
        LocationType CurrentLocation { get; }
    }
}