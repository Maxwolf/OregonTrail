using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Group of persons and the vehicle they are traveling in. Name is hard-coded to always be player party since what
    ///     matters in it's identification. Players will never see the name of their party, it is used purely for internal
    ///     logic reasons.
    /// </summary>
    public class Party : EntityBase, IParty
    {
        /// <summary>
        ///     People that make up every member of the party.
        /// </summary>
        private List<IPerson> _members;

        /// <summary>
        ///     Current location in the game world as defined by object that represents it.
        /// </summary>
        private ILocation _partyLocation;

        /// <summary>
        ///     Amount of food people in party eat each day can change.
        /// </summary>
        private FoodRations _partyRations;

        /// <summary>
        ///     The pace at which you travel can change.
        /// </summary>
        private TravelPace _travelPace;

        /// <summary>
        ///     Vehicle which the party members are traveling in.
        /// </summary>
        private Vehicle _vessel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Party" /> class.
        /// </summary>
        public Party(List<IPerson> members, Vehicle vessel, ILocation partyLocation)
            : base(ConditionTier.Good, "Player Party")
        {
            _members = members;
            _vessel = vessel;
            _partyLocation = partyLocation;
            _partyRations = FoodRations.Filling;
            _travelPace = TravelPace.Steady;
        }

        /// <summary>
        ///     People that make up every member of the party.
        /// </summary>
        public ReadOnlyCollection<IPerson> Members
        {
            get { return new ReadOnlyCollection<IPerson>(_members); }
        }

        /// <summary>
        ///     Vehicle which the party members are traveling in.
        /// </summary>
        public IVehicle Vessel
        {
            get { return _vessel; }
        }

        /// <summary>
        ///     Current location in the game world as defined by object that represents it.
        /// </summary>
        public ILocation PartyLocation
        {
            get { return _partyLocation; }
        }

        /// <summary>
        ///     Amount of food people in party eat each day can change.
        /// </summary>
        public FoodRations PartyRations
        {
            get { return _partyRations; }
        }

        /// <summary>
        ///     The pace at which you travel can change.
        /// </summary>
        public TravelPace TravelPace
        {
            get { return _travelPace; }
        }
    }
}