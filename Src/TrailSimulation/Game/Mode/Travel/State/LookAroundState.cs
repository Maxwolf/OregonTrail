using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     State that is attached when the event is fired for reaching a new point of interest on the trail. Default action is
    ///     to ask the player if they would like to look around, but there is a chance for this behavior to be overridden in
    ///     the constructor there is a default boolean value to skip the question asking part and force a look around event to
    ///     occur without player consent.
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
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get
            {
                // First location we only tell the user they are going back in time and arrived at first location on trail.
                return GameSimulationApp.Instance.Trail.IsFirstLocation
                    ? DialogType.Prompt
                    : DialogType.YesNo;
            }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var pointReached = new StringBuilder();
            if (GameSimulationApp.Instance.Trail.IsFirstLocation)
            {
                // First point of interest has slightly different message about time travel.
                pointReached.AppendLine(
                    $"{Environment.NewLine}Going back to {GameSimulationApp.Instance.Time.CurrentYear}...{Environment.NewLine}");
            }
            else
            {
                // Build up message about location the player is arriving at.
                pointReached.AppendLine(
                    $"{Environment.NewLine}You are now at the {GameSimulationApp.Instance.Trail.CurrentLocation.Name}.");
                pointReached.Append("Would you like to look around? Y/N");
            }

            // Wait for input on deciding if we should take a look around.
            return pointReached.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // First location we will always clear state back to location since it is starting point.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation)
            {
                ClearState();
                return;
            }

            // Subsequent locations will confirm what the player wants to do, they can stop or keep going on the trail at their own demise.
            switch (reponse)
            {
                case DialogResponse.Custom:
                case DialogResponse.No:
                    // Cast the parent game mode as travel mode.
                    var travelMode = ParentMode as TravelMode;
                    if (travelMode == null)
                        throw new InvalidCastException(
                            "Unable to cast parent game mode into travel game mode when it should be that!");

                    // Call the continue on trail method command inside that game mode, it will trigger the next action accordingly.
                    travelMode.ContinueOnTrail();
                    break;
                case DialogResponse.Yes:
                    // Clearing this state will drop back to travel mode with options for the player to choose from.
                    ClearState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}