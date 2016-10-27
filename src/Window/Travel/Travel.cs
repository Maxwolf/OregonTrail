// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Dialog;
using OregonTrailDotNet.Window.Travel.Hunt.Help;
using OregonTrailDotNet.Window.Travel.Rest;
using OregonTrailDotNet.Window.Travel.RiverCrossing.Help;
using OregonTrailDotNet.Window.Travel.Store.Help;
using OregonTrailDotNet.Window.Travel.Trade;
using WolfCurses;
using WolfCurses.Window;

namespace OregonTrailDotNet.Window.Travel
{
    /// <summary>
    ///     Primary game Windows used for advancing simulation down the trail.
    /// </summary>
    public sealed class Travel : Window<TravelCommands, TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        public Travel(SimulationApp simUnit) : base(simUnit)
        {
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
            SetForm(typeof(TalkToPeople.TalkToPeople));
        }

        /// <summary>
        ///     Attached store game Windows on top of existing game Windows for purchasing items from this location.
        /// </summary>
        private void BuySupplies()
        {
            SetForm(typeof(Store.Store));
        }

        /// <summary>
        ///     Resumes the simulation and progression down the trail towards the next point of interest.
        /// </summary>
        internal void ContinueOnTrail()
        {
            // Check if player has already departed and we are just moving along again.
            if (GameSimulationApp.Instance.Trail.CurrentLocation.Status == LocationStatus.Departed)
            {
                SetForm(typeof(ContinueOnTrail));
                return;
            }

            // Depending on what kind of location we are heading towards we will invoke different forms.
            if (GameSimulationApp.Instance.Trail.CurrentLocation is Landmark ||
                GameSimulationApp.Instance.Trail.CurrentLocation is Settlement ||
                GameSimulationApp.Instance.Trail.CurrentLocation is TollRoad)
                SetForm(typeof(LocationDepart));
            else if (GameSimulationApp.Instance.Trail.CurrentLocation is Entity.Location.Point.RiverCrossing)
                SetForm(typeof(RiverCrossHelp));
            else if (GameSimulationApp.Instance.Trail.CurrentLocation is ForkInRoad)
                SetForm(typeof(LocationFork));
        }

        /// <summary>
        ///     Shows current load out for vehicle and player inventory items.
        /// </summary>
        private void CheckSupplies()
        {
            SetForm(typeof(CheckSupplies));
        }

        /// <summary>
        ///     Shows players current position on the total trail along with progress indicators so they know how much more they
        ///     have and what they have accomplished.
        /// </summary>
        private void LookAtMap()
        {
            SetForm(typeof(LookAtMap));
        }

        /// <summary>
        ///     Changes the number of miles the vehicle will attempt to move in a single day.
        /// </summary>
        private void ChangePace()
        {
            SetForm(typeof(ChangePace));
        }

        /// <summary>
        ///     Changes the amount of food in pounds the vehicle party members will consume each day of the simulation.
        /// </summary>
        private void ChangeFoodRations()
        {
            SetForm(typeof(ChangeRations));
        }

        /// <summary>
        ///     Attaches state that will ask how many days should be ticked while sitting still, if zero is entered then nothing
        ///     happens.
        /// </summary>
        private void StopToRest()
        {
            SetForm(typeof(RestAmount));
        }

        /// <summary>
        ///     Looks through the traveling information data for any pending trades that people might want to make with you.
        /// </summary>
        private void AttemptToTrade()
        {
            SetForm(typeof(Trading));
        }

        /// <summary>
        ///     Attaches a new Windows on top of this one that allows the player to hunt for animals and kill them using bullets
        ///     for a specified time limit.
        /// </summary>
        private void HuntForFood()
        {
            // Check if the player even has enough bullets to go hunting.
            SetForm(GameSimulationApp.Instance.Vehicle.Inventory[Entities.Ammo].Quantity > 0
                ? typeof(HuntingPrompt)
                : typeof(NoAmmo));
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
                    break;
                case LocationStatus.Arrived:
                    AddCommand(AttemptToTrade, TravelCommands.AttemptToTrade);

                    // Some commands are optional and change depending on location category.
                    if (location.ChattingAllowed)
                        AddCommand(TalkToPeople, TravelCommands.TalkToPeople);

                    if (location.ShoppingAllowed)
                        AddCommand(BuySupplies, TravelCommands.BuySupplies);
                    break;
                case LocationStatus.Departed:
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
                (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached))
                SetForm(typeof(StoreWelcome));
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

            // Check if passengers in the vehicle are dead, or player reached end of the trail.
            if (game.Trail.CurrentLocation.LastLocation || game.Vehicle.PassengersDead)
            {
                GameOver = true;
                game.WindowManager.Add(typeof(GameOver.GameOver));
                return;
            }

            // Check if player is just arriving at a new location.
            if ((game.Trail.CurrentLocation.Status == LocationStatus.Arrived) && !game.Trail.CurrentLocation.ArrivalFlag &&
                !GameOver)
            {
                game.Trail.CurrentLocation.ArrivalFlag = true;
                SetForm(typeof(LocationArrive));
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
            // Skip if the player has already arrived at this location.
            if (!GameSimulationApp.Instance.Trail.CurrentLocation.ArrivalFlag)
                return;

            ArriveAtLocation();
        }
    }
}