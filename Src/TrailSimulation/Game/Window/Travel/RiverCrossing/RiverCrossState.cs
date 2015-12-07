using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Utility;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    [RequiredWindow(Windows.Travel)]
    public sealed class RiverCrossState : Form<TravelInfo>
    {
        /// <summary>
        ///     Holds all the information about the river and crossing decisions so it only needs to be constructed once at
        ///     startup.
        /// </summary>
        private StringBuilder _riverInfo;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RiverCrossState(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Header text for above menu comes from river crossing info object.
            _riverInfo = new StringBuilder();
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation.Name}");
            _riverInfo.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine(
                $"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather.ToDescriptionAttribute()}");
            _riverInfo.AppendLine($"River width: {UserData.River.RiverWidth.ToString("N0")} feet");
            _riverInfo.AppendLine($"River depth: {UserData.River.RiverDepth.ToString("N0")} feet");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"You may:{Environment.NewLine}");

            // Loop through all the river choice commands and print them out for the state.
            var choices = new List<RiverCrossChoice>(Enum.GetValues(typeof (RiverCrossChoice)).Cast<RiverCrossChoice>());
            for (var index = 1; index < choices.Count; index++)
            {
                // Get the current river choice enumeration value we casted into list.
                var riverChoice = choices[index];

                // Last line should not print new line.
                if (index == (choices.Count - 1))
                {
                    _riverInfo.Append((int) riverChoice + ". " + riverChoice.ToDescriptionAttribute());
                }
                else
                {
                    _riverInfo.AppendLine((int) riverChoice + ". " + riverChoice.ToDescriptionAttribute());
                }
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            return _riverInfo.ToString();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
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
                case RiverCrossChoice.Ford:
                    UserData.River.CrossingType = RiverCrossChoice.Ford;
                    SetForm(typeof (CrossingResultState));
                    break;
                case RiverCrossChoice.Float:
                    UserData.River.CrossingType = RiverCrossChoice.Float;
                    SetForm(typeof (CrossingResultState));
                    break;
                case RiverCrossChoice.Ferry:
                    UserData.River.CrossingType = RiverCrossChoice.Ferry;
                    SetForm(typeof (UseFerryConfirmState));
                    break;
                case RiverCrossChoice.WaitForWeather:
                    // Resting by a river only increments a single day at a time.
                    UserData.DaysToRest = 1;
                    UserData.River.CrossingType = RiverCrossChoice.WaitForWeather;
                    SetForm(typeof (RestingState));
                    break;
                case RiverCrossChoice.GetMoreInformation:
                    SetForm(typeof (FordRiverHelpState));
                    break;
            }
            // ReSharper restore SwitchStatementMissingSomeCases
        }
    }
}