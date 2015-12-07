using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Keeps track of a set number of days and every time the game mode is ticked a day is simulated and days to rest
    ///     subtracted until we are at zero, then the player can close the window but until then input will not be accepted.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class RestingState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     References the number of days the player has reseted, this ticks up each time we rest a day and will be used for
        ///     display purposes to user.
        /// </summary>
        private int _daysRested;

        /// <summary>
        ///     Holds the message that is printed to the text renderer for debugging about the number of days rested.
        /// </summary>
        private StringBuilder _restMessage;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RestingState(IModeProduct gameMode) : base(gameMode)
        {
            _restMessage = new StringBuilder();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        public override void OnTick(bool systemTick)
        {
            base.OnTick(systemTick);

            // Skip system ticks.
            if (systemTick)
                return;

            // Not ticking when days to rest is zero.
            if (UserData.DaysToRest <= 0)
                return;

            // Check if we are at a river crossing and need to subtract from ferry days also.
            if (UserData.River != null &&
                UserData.River.FerryDelayInDays > 0 &&
                GameSimulationApp.Instance.Trail.CurrentLocation.Category == LocationCategory.RiverCrossing)
                UserData.River.FerryDelayInDays--;

            // Decrease number of days needed to rest, increment number of days rested.
            UserData.DaysToRest--;

            // Increment the number of days reseted.
            _daysRested++;

            // Simulate the days to rest in time and event system, this will trigger random event game mode if required.
            GameSimulationApp.Instance.TakeTurn();
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnStatePostCreate()
        {
            base.OnStatePostCreate();

            // Vehicle should be stopped while resting to prevent it from moving during ticks.
            GameSimulationApp.Instance.Vehicle.Status = VehicleStatus.Stopped;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            // String that holds message about resting, it can change depending on location.
            _restMessage.Clear();

            // Change up resting prompt depending on location category to give it some context.
            switch (GameSimulationApp.Instance.Trail.CurrentLocation.Category)
            {
                case LocationCategory.RiverCrossing:
                    // Ferry operator can request you rest or player decides to wait out weather conditions.
                    if (_daysRested > 1)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}You camp near the river for {_daysRested} days.");
                    }
                    else if (_daysRested == 1)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}You camp near the river for a day.");
                    }
                    else if (_daysRested <= 0)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}Preparing to camp near the river...");
                    }
                    break;
                case LocationCategory.Landmark:
                case LocationCategory.Settlement:
                case LocationCategory.ForkInRoad:
                    // Normal resting message just says time rested.
                    if (_daysRested > 1)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}You rest for {_daysRested} days");
                    }
                    else if (_daysRested == 1)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}You rest for a day.");
                    }
                    else if (_daysRested <= 0)
                    {
                        _restMessage.AppendLine($"{Environment.NewLine}Preparing to rest...");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Allow the user to stop resting, this will break the cycle and reset days to rest to zero.
            if (_daysRested > 0)
                _restMessage.AppendLine($"{Environment.NewLine}Press ENTER to stop resting.{Environment.NewLine}");

            // Prints out the message about resting for however long this cycle was.
            return _restMessage.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Figure out what to do with response.
            if (_daysRested > 0)
                StopResting();
        }

        /// <summary>
        ///     Forces the resting period to end and days to rest reset to zero even if there was time remaining.
        /// </summary>
        private void StopResting()
        {
            // Determine if we should bounce back to travel menu or some special mode.
            if (UserData.River == null)
            {
                ClearState();
                return;
            }

            // Locations can return to a special state if required based on the category of the location.
            switch (GameSimulationApp.Instance.Trail.CurrentLocation.Category)
            {
                case LocationCategory.Landmark:
                case LocationCategory.Settlement:
                    // Player is going to go back to travel mode now.
                    ClearState();
                    break;
                case LocationCategory.RiverCrossing:
                    // Reset the days to rest to zero, ferry operator adds to this value.
                    UserData.DaysToRest = 0;

                    // Player might be crossing a river, so we check if they made a decision and are waiting for ferry operator.
                    if (UserData.River != null &&
                        UserData.River.CrossingType == RiverCrossChoice.Ferry &&
                        UserData.River.FerryDelayInDays <= 0 &&
                        UserData.River.FerryCost >= 0)
                    {
                        // If player was waiting for ferry operator to let them cross we will jump right to that.
                        SetState(typeof (CrossingResultState));
                    }
                    else
                    {
                        // Alternative is player just was waiting for weather.
                        SetState(typeof (RiverCrossState));
                    }
                    break;
                case LocationCategory.ForkInRoad:
                    // Player needs to decide on which location when road splits.
                    SetState(typeof (ForkInRoadState));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}