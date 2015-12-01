using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class RiverCrossState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     Holds all the information about the river and crossing decisions so it only needs to be constructed once at
        ///     startup.
        /// </summary>
        private StringBuilder _riverInfo;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RiverCrossState(IModeProduct gameMode) : base(gameMode)
        {
            // Header text for above menu comes from river crossing info object.
            _riverInfo = new StringBuilder();
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation.Name}");
            _riverInfo.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}");
            _riverInfo.AppendLine($"River width: {UserData.RiverInfo.RiverWidth} feet");
            _riverInfo.AppendLine($"River depth: {UserData.RiverInfo.RiverDepth} feet");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"You may:{Environment.NewLine}");

            // Loop through all the river choice commands and print them out for the state.
            var choices = new List<RiverCrossChoice>(Enum.GetValues(typeof (RiverCrossChoice)).Cast<RiverCrossChoice>());
            for (var index = 0; index < choices.Count; index++)
            {
                // Get the current river choice enumeration value we casted into list.
                var riverChoice = choices[index];

                // Last line should not print new line.
                if (index == (choices.Count-1))
                {
                    _riverInfo.Append((int) riverChoice + ". " + riverChoice.ToDescriptionAttribute());
                }
                else
                {
                    _riverInfo.AppendLine((int) riverChoice + ". " + riverChoice.ToDescriptionAttribute());
                }
            }

            // Add the state that explains the player is at a river crossing and what is expected of them.
            SetState(typeof (RiverPromptState));
        }

        /// <summary>
        ///     Attached a state on top of the river crossing mode to explain what the different options mean and how they work.
        /// </summary>
        private void GetMoreInformation()
        {
            SetState(typeof (FordRiverHelpState));
        }

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        private void WaitForWeather()
        {
            SetState(typeof (CampByRiverState));
        }

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        private void UseFerry()
        {
            UserData.RiverInfo.CrossingType = RiverCrossChoice.UseFerry;
            SetState(typeof (UseFerryConfirmState));
        }

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        private void CaulkVehicle()
        {
            UserData.RiverInfo.CrossingType = RiverCrossChoice.CaulkVehicle;
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        private void FordRiver()
        {
            UserData.RiverInfo.CrossingType = RiverCrossChoice.FordRiver;
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _riverInfo.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            RiverCrossChoice riverChoice;
            Enum.TryParse(input, out riverChoice);

            // Depending on selection made we will decide on what to do.
            // ReSharper disable SwitchStatementMissingSomeCases
            switch (riverChoice)
            {
                case RiverCrossChoice.FordRiver:
                    FordRiver();
                    break;
                case RiverCrossChoice.CaulkVehicle:
                    CaulkVehicle();
                    break;
                case RiverCrossChoice.UseFerry:
                    UseFerry();
                    break;
                case RiverCrossChoice.WaitForWeather:
                    WaitForWeather();
                    break;
                case RiverCrossChoice.GetMoreInformation:
                    GetMoreInformation();
                    break;
            }
            // ReSharper restore SwitchStatementMissingSomeCases
        }
    }
}