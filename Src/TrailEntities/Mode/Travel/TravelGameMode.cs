using System.Text;
using TrailEntities.Simulation;
using TrailEntities.State;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Primary game gameMode of the simulation, used to show simulation advancing through linear time. Shows all major stats
    ///     of party and vehicle, plus climate and other things like distance traveled and distance to next point.
    /// </summary>
    public sealed class TravelGameMode : ModeProduct
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct.TravelGameMode" /> class.
        /// </summary>
        public TravelGameMode() : base(false)
        {
            // Keep track of basic information about menu choices, vehicle and party stats, trades, advice, etc.
            TravelInfo = new TravelInfo();

            // Update menu with proper choices.
            UpdateLocation();
        }

        /// <summary>
        ///     Traveling game gameMode has a gameMode state information object that is used to keep track of any important info about the
        ///     state like how many days we should rest.
        /// </summary>
        private TravelInfo TravelInfo { get; }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.Travel; }
        }

        /// <summary>
        ///     Attaches state that picks strings from array at random to show from point of interest.
        /// </summary>
        private void TalkToPeople()
        {
            AddState(typeof(TalkToPeopleState));
        }

        /// <summary>
        ///     Attached store game gameMode on top of existing game gameMode for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            GameSimApp.Instance.AttachMode(GameMode.Store);
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        private void ContinueOnTrail()
        {
            // Player just starting this section of the trail will get prompt about total distance needed to cover it before starting.
            if (!GameSimApp.Instance.Trail.ReachedNextPoint() && !GameSimApp.Instance.Trail.IsFirstLocation())
            {
                AddState(typeof(DriveState));
            }
            else
            {
                AddState(typeof(ContinueOnTrailState));
            }
        }

        /// <summary>
        ///     Shows current load out for vehicle and player inventory items.
        /// </summary>
        private void CheckSupplies()
        {
            AddState(typeof(CheckSuppliesState));
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            AddState(typeof(LookAtMapState));
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            AddState(typeof(ChangePaceState));
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            AddState(typeof(ChangeRationsState));
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            AddState(typeof(RestQuestionState));
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            RemoveState();
            GameSimApp.Instance.AttachMode(GameMode.Trade);
        }

        /// <summary>
        ///     Attaches a new gameMode on top of this one that allows the player to hunt for animals and kill them using bullets for a
        ///     specified time limit.
        /// </summary>
        private void HuntForFood()
        {
            RemoveState();
            GameSimApp.Instance.AttachMode(GameMode.Hunt);
        }

        /// <summary>
        ///     Determines if there is a store, people to get advice from, and a place to rest, what options are available, etc.
        /// </summary>
        private void UpdateLocation()
        {
            // Header text for above menu comes from travel info object.
            var headerText = new StringBuilder();
            headerText.Append(TravelInfo.TravelStatus(GameSimApp.Instance.Trail.ReachedNextPoint()));
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            // Reset and calculate what commands are allowed at this current point of interest on the trail.
            ClearCommands();
            AddCommand(ContinueOnTrail, TravelCommands.ContinueOnTrail);
            AddCommand(CheckSupplies, TravelCommands.CheckSupplies);
            AddCommand(LookAtMap, TravelCommands.LookAtMap);
            AddCommand(ChangePace, TravelCommands.ChangePace);
            AddCommand(ChangeFoodRations, TravelCommands.ChangeFoodRations);
            AddCommand(StopToRest, TravelCommands.StopToRest);
            AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade);
            AddCommand(TalkToPeople, TravelCommands.TalkToPeople);
            AddCommand(BuySupplies, TravelCommands.BuySupplies);
            AddCommand(HuntForFood, TravelCommands.HuntForFood);
        }

        /// <summary>
        ///     Fired when trail simulation has determined the vehicle and player party has reached the next point of interest in
        ///     the trail.
        /// </summary>
        /// <param name="nextPoint"></param>
        protected override void OnReachNextLocation(Location nextPoint)
        {
            base.OnReachNextLocation(nextPoint);

            // On the first point we are going to force the look around state onto the traveling gameMode without asking.
            if (GameSimApp.Instance.Trail.IsFirstLocation())
            {
                AddState(typeof(LookAroundState));
            }
            else if (!GameSimApp.Instance.Trail.IsFirstLocation() &&
                     GameSimApp.Instance.Vehicle.Odometer > 0 &&
                     GameSimApp.Instance.TotalTurns > 0)
            {
                // Ensure we only ask if the player wants to stop when it is really not the first turn.
                AddState(typeof(LookAroundQuestionState));
            }
        }
    }
}