using System;
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
        public override Mode Mode
        {
            get { return Mode.Travel; }
        }

        /// <summary>
        ///     Attaches state that picks strings from array at random to show from point of interest.
        /// </summary>
        private void TalkToPeople()
        {
            SetState(typeof (TalkToPeopleState));
        }

        /// <summary>
        ///     Attached store game mode on top of existing game mode for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            GameSimulationApp.Instance.ModeManager.AddMode(Mode.Store);
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        private void ContinueOnTrail()
        {
            switch (GameSimulationApp.Instance.Trail.CurrentLocation.Status)
            {
                case LocationStatus.Unreached:
                    // Player has not reached the next location so we return to the drive state to reduce distance to it.
                    SetState(typeof (DriveState));
                    break;
                case LocationStatus.Arrived:
                    switch (GameSimulationApp.Instance.Trail.CurrentLocation.Category)
                    {
                        case LocationCategory.Landmark:
                        case LocationCategory.Settlement:
                            // Tell player how far it is to next location before attaching drive state.
                            SetState(typeof (ContinueOnTrailDialog));
                            break;
                        case LocationCategory.RiverCrossing:
                            // Player needs to decide how to cross a river.
                            SetState(typeof (RiverPromptState));
                            break;
                        case LocationCategory.ForkInRoad:
                            // Player needs to decide on which location when road splits.
                            SetState(typeof (ForkInRoadState));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case LocationStatus.Departed:
                    // Look around state, player, or location triggered departure.
                    SetState(typeof (DriveState));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Shows current load out for vehicle and player inventory items.
        /// </summary>
        private void CheckSupplies()
        {
            SetState(typeof (CheckSuppliesDialog));
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            SetState(typeof (LookAtMapDialog));
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            SetState(typeof (ChangePaceState));
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            SetState(typeof (ChangeRationsState));
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            SetState(typeof (RestQuestionState));
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            ClearState();
            GameSimulationApp.Instance.ModeManager.AddMode(Mode.Trade);
        }

        /// <summary>
        ///     Attaches a new mode on top of this one that allows the player to hunt for animals and kill them using bullets for a
        ///     specified time limit.
        /// </summary>
        private void HuntForFood()
        {
            ClearState();
            GameSimulationApp.Instance.ModeManager.AddMode(Mode.Hunt);
        }

        /// <summary>
        ///     Determines if there is a store, people to get advice from, and a place to rest, what options are available, etc.
        /// </summary>
        private void UpdateLocation()
        {
            // Header text for above menu comes from travel info object.
            var headerText = new StringBuilder();
            headerText.Append(TravelInfo.TravelStatus);
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            // Get a reference to current location on the trail so we can query it to build our menu.
            var location = GameSimulationApp.Instance.Trail.CurrentLocation;

            // Reset and calculate what commands are allowed at this current point of interest on the trail.
            ClearCommands();
            AddCommand(ContinueOnTrail, TravelCommands.ContinueOnTrail);
            AddCommand(CheckSupplies, TravelCommands.CheckSupplies);
            AddCommand(LookAtMap, TravelCommands.LookAtMap);
            AddCommand(ChangePace, TravelCommands.ChangePace);
            AddCommand(ChangeFoodRations, TravelCommands.ChangeFoodRations);
            AddCommand(StopToRest, TravelCommands.StopToRest);

            // Some commands are optional and change depending on location category.
            if (location.TradingAllowed)
                AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade);

            if (location.ChattingAllowed)
                AddCommand(TalkToPeople, TravelCommands.TalkToPeople);

            if (location.ShoppingAllowed)
                AddCommand(BuySupplies, TravelCommands.BuySupplies);

            if (location.HuntingAllowed)
                AddCommand(HuntForFood, TravelCommands.HuntForFood);
        }

        /// <summary>
        ///     Fired when the game mode changes it's internal state. Allows the attached mode to do special behaviors when it
        ///     realizes a state is set or removed and act on it.
        /// </summary>
        protected override void OnStateChange()
        {
            base.OnStateChange();

            // Update menu with proper choices.
            UpdateLocation();
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
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        public override void OnModeActivate()
        {
            CheckLookAround();
        }

        /// <summary>
        ///     On the first point we are going to force the look around state onto the traveling mode without asking.
        /// </summary>
        private void CheckLookAround()
        {
            // Update menu with proper choices.
            UpdateLocation();

            SetState(GameSimulationApp.Instance.Trail.IsFirstLocation
                ? typeof (LookAroundState)
                : typeof (LookAroundQuestionState));
        }

        /// <summary>
        ///     Fired when the simulation adds a game mode that is not this mode. Used to execute code in other modes that are not
        ///     the active mode anymore one last time.
        /// </summary>
        public override void OnModeAdded()
        {
            CheckLookAround();
        }
    }
}