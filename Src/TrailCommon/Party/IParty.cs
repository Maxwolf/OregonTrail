using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Party is a defined list of persons entities that make up the party member property. Vessel is a entity that all the
    ///     party travels in and has it's own health and parts that make it operate.
    /// </summary>
    public interface IParty : IEntity
    {
        /// <summary>
        ///     People that make up every member of the party.
        /// </summary>
        ReadOnlyCollection<IPerson> Members { get; }

        /// <summary>
        ///     Vehicle which the party members are traveling in.
        /// </summary>
        IVehicle Vessel { get; }

        /// <summary>
        ///     Current location in the game world as defined by object that represents it.
        /// </summary>
        ILocation PartyLocation { get; }

        /// <summary>
        ///     Amount of food people in party eat each day can change.
        /// </summary>
        FoodRations PartyRations { get; }

        /// <summary>
        ///     The pace at which you travel can change.
        /// </summary>
        TravelPace TravelPace { get; }
    }
}