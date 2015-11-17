using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Primary game mode of the simulation, used to show simulation advancing through linear time. Shows all major stats
    ///     of party and vehicle, plus climate and other things like distance traveled and distance to next point.
    /// </summary>
    public sealed class TravelMode : ModeProduct<TravelCommands, TravelInfo>
    {
        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode GameMode
        {
            get { return GameMode.Travel; }
        }

        /// <summary>
        ///     Attaches state that picks strings from array at random to show from point of interest.
        /// </summary>
        private void TalkToPeople()
        {
            //CurrentState = new TalkToPeopleState(this, TravelInfo);
            SetState(typeof (TalkToPeopleState));
        }

        /// <summary>
        ///     Attached store game mode on top of existing game mode for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.Store);
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        private void ContinueOnTrail()
        {
            // Player just starting this section of the trail will get prompt about total distance needed to cover it before starting.
            if (!GameSimulationApp.Instance.Trail.ReachedNextPoint() &&
                !GameSimulationApp.Instance.Trail.IsFirstLocation())
            {
                //CurrentState = new DriveState(this, TravelInfo);
                SetState(typeof (DriveState));
            }
            else
            {
                //CurrentState = new ContinueOnTrailState(this, TravelInfo);
                SetState(typeof (ContinueOnTrailState));
            }
        }

        /// <summary>
        ///     Shows current load out for vehicle and player inventory items.
        /// </summary>
        private void CheckSupplies()
        {
            //CurrentState = new CheckSuppliesState(this, TravelInfo);
            SetState(typeof (CheckSuppliesState));
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            //CurrentState = new LookAtMapState(this, TravelInfo);
            SetState(typeof (LookAtMapState));
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            //CurrentState = new ChangePaceState(this, TravelInfo);
            SetState(typeof (ChangePaceState));
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            //CurrentState = new ChangeRationsState(this, TravelInfo);
            SetState(typeof (ChangeRationsState));
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            //CurrentState = new RestQuestionState(this, TravelInfo);
            SetState(typeof (RestQuestionState));
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            //CurrentState = null;
            ClearState();
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.Trade);
        }

        /// <summary>
        ///     Attaches a new mode on top of this one that allows the player to hunt for animals and kill them using bullets for a
        ///     specified time limit.
        /// </summary>
        private void HuntForFood()
        {
            //CurrentState = null;
            ClearState();
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.Hunt);
        }

        /// <summary>
        ///     Determines if there is a store, people to get advice from, and a place to rest, what options are available, etc.
        /// </summary>
        private void UpdateLocation()
        {
            // Header text for above menu comes from travel info object.
            var headerText = new StringBuilder();
            headerText.Append(UserData.TravelStatus(GameSimulationApp.Instance.Trail.ReachedNextPoint()));
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
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
            // Update menu with proper choices.
            UpdateLocation();
        }

        /// <summary>
        ///     Fired when trail simulation has determined the vehicle and player party has reached the next point of interest in
        ///     the trail.
        /// </summary>
        /// <param name="nextPoint"></param>
        protected override void OnReachNextLocation(Location nextPoint)
        {
            base.OnReachNextLocation(nextPoint);

            // On the first point we are going to force the look around state onto the traveling mode without asking.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation())
            {
                SetState(typeof (LookAroundState));
            }
            else if (!GameSimulationApp.Instance.Trail.IsFirstLocation() &&
                     GameSimulationApp.Instance.Vehicle.Odometer > 0 &&
                     GameSimulationApp.Instance.TotalTurns > 0)
            {
                // Ensure we only ask if the player wants to stop when it is really not the first turn.
                SetState(typeof (LookAroundQuestionState));
            }
        }
    }
}