using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Primary game Windows of the simulation, used to show simulation advancing through linear time. Shows all major
    ///     stats of party and vehicle, plus climate and other things like distance traveled and distance to next point.
    /// </summary>
    public sealed class Travel : Window<TravelCommands, TravelInfo>
    {
        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow WindowCategory
        {
            get { return GameWindow.Travel; }
        }

        /// <summary>
        ///     Determines if the simulation should continue to check if the game has ended.
        /// </summary>
        private bool GameOver { get; set; }

        /// <summary>
        ///     Attaches state that picks strings from array at random to show from point of interest.
        /// </summary>
        private void TalkToPeople()
        {
            SetForm(typeof (TalkToPeople));
        }

        /// <summary>
        ///     Attached store game Windows on top of existing game Windows for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            SetForm(typeof (Store));
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        internal void ContinueOnTrail()
        {
            // Check if the vehicle is stuck and unable to continue.
            if (GameSimulationApp.Instance.Vehicle.Status == VehicleStatus.Stuck)
            {
                SetForm(typeof (VehicleStuck));
                return;
            }

            // Check if player has already departed and we are just moving along again.
            if (GameSimulationApp.Instance.Trail.CurrentLocation.Status == LocationStatus.Departed)
            {
                SetForm(typeof (ContinueOnTrail));
                return;
            }

            // If we have not departed the current location yet there are several things we could need to do in order to depart.
            switch (GameSimulationApp.Instance.Trail.CurrentLocation.Category)
            {
                case LocationCategory.Landmark:
                case LocationCategory.Settlement:
                    // Player is going to continue driving down the trail now.
                    SetForm(typeof (LocationDepart));
                    break;
                case LocationCategory.RiverCrossing:
                    // Player needs to decide how to cross a river.
                    SetForm(typeof (RiverCrossHelp));
                    break;
                case LocationCategory.ForkInRoad:
                    // Player needs to decide on which location when road splits.
                    SetForm(typeof (LocationFork));
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
            SetForm(typeof (CheckSupplies));
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            SetForm(typeof (LookAtMap));
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            SetForm(typeof (ChangePace));
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            SetForm(typeof (ChangeRations));
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            SetForm(typeof (RestAmount));
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            SetForm(typeof (Trading));
        }

        /// <summary>
        ///     Attaches a new Windows on top of this one that allows the player to hunt for animals and kill them using bullets
        ///     for a specified time limit.
        /// </summary>
        private void HuntForFood()
        {
            SetForm(typeof (Hunting));
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

            // Depending on where you are at on the trail the last few available commands change.
            switch (location.Status)
            {
                case LocationStatus.Unreached:
                    // Setup phase of the game before you are placed on the first location.
                    break;
                case LocationStatus.Arrived:
                    // Can always attempt to trade, probability is good ones is way less outside settlements.
                    AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade);

                    // Some commands are optional and change depending on location category.
                    if (location.ChattingAllowed)
                        AddCommand(TalkToPeople, TravelCommands.TalkToPeople);

                    if (location.ShoppingAllowed)
                        AddCommand(BuySupplies, TravelCommands.BuySupplies);
                    break;
                case LocationStatus.Departed:
                    // Some commands can only be done when between locations.
                    AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade);
                    AddCommand(HuntForFood, TravelCommands.HuntForFood);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Fired when the game Windows changes it's internal state. Allows the attached Windows to do special behaviors when
        ///     it realizes a state is set or removed and act on it.
        /// </summary>
        protected override void OnFormChange()
        {
            base.OnFormChange();

            // Update menu with proper choices.
            UpdateLocation();
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            // Update menu with proper choices.
            UpdateLocation();

            // Starting store that is shown after setting up player names, profession, and starting month.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached)
            {
                // Calculate initial distance to next point.
                SetForm(typeof (StoreWelcome));
            }
        }

        /// <summary>
        ///     Called when the Windows manager in simulation makes this Windows the currently active game Windows. Depending on
        ///     order of modes this might not get called until the Windows is actually ticked by the simulation.
        /// </summary>
        public override void OnWindowActivate()
        {
            ArriveAtLocation();
        }

        /// <summary>
        ///     On the first point we are going to force the look around state onto the traveling Windows without asking.
        /// </summary>
        private void ArriveAtLocation()
        {
            // Grab instance of game simulation for easy reading.
            var game = GameSimulationApp.Instance;

            // Skip ticking logic for travel mode if game is closing.
            if (game.IsClosing)
                return;

            // Skip if we have already ended the game.
            if (GameOver)
                return;

            // Check if the player made it all the way to the end of the trail.
            if (game.Trail.CurrentLocation.IsLast)
            {
                GameOver = true;
                SetForm(typeof (GameWin));
                return;
            }

            // Determines if all the passengers in the vehicle are dead, this does not apply to first location.
            if (game.Vehicle.PassengersDead)
            {
                GameOver = true;
                SetForm(typeof (GameFail));
                return;
            }

            // Check if player is just arriving at a new location.
            if (game.Trail.CurrentLocation.Status == LocationStatus.Arrived &&
                !game.Trail.CurrentLocation.ArrivalFlag && !GameOver)
            {
                game.Trail.CurrentLocation.ArrivalFlag = true;
                SetForm(typeof (LocationArrive));
                return;
            }

            // Update menu with proper choices.
            UpdateLocation();
        }

        /// <summary>
        ///     Fired when the simulation adds a game Windows that is not this Windows. Used to execute code in other modes that
        ///     are not the active Windows anymore one last time.
        /// </summary>
        public override void OnWindowAdded()
        {
            ArriveAtLocation();
        }
    }
}