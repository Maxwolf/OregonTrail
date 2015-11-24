using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Tells the player about the point of interest they have just arrived at if they choose to look at it. Some locations
    ///     like the first location will force the player to look around regardless if they want to or not and print a
    ///     different message about traveling back in time.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class LookAroundState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAroundState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var welcomePoint = new StringBuilder();
            if (GameSimulationApp.Instance.Trail.IsFirstLocation)
            {
                // First point of interest has slightly different message about time travel.
                welcomePoint.AppendLine(
                    $"Going back to {GameSimulationApp.Instance.Time.CurrentYear}...{Environment.NewLine}");
            }
            else
            {
                // Every other point of interest will say the name and current date.
                welcomePoint.AppendLine(GameSimulationApp.Instance.Trail.CurrentLocation.Name);
                welcomePoint.AppendLine(GameSimulationApp.Instance.Time.Date.ToString());
            }

            return welcomePoint.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearState();
        }
    }
}