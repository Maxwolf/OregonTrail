using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Attached when the player wants to continue on the trail, and doing so will force them to leave that point and be
    ///     back on the trail counting up distance traveled until they reach the next one. The purpose of this state is to
    ///     inform the player of the next points name, the distance away that it is, and that is all it will close and
    ///     simulation resume after return key is pressed.
    /// </summary>
    public sealed class ContinueOnTrailState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ContinueOnTrailState(IModeProduct gameMode, TravelInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.Prompt; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var nextStop = new StringBuilder();
            var nextPoint = GameSimulationApp.Instance.Trail.GetNextLocation();
            nextStop.Append(
                $"{Environment.NewLine}From {ParentMode.CurrentPoint.Name} it is {GameSimulationApp.Instance.Trail.DistanceToNextLocation}{Environment.NewLine}");
            nextStop.Append($"miles to the {nextPoint.Name}{Environment.NewLine}{Environment.NewLine}");

            return nextStop.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            UserData.HasLookedAround = false;
            //ParentMode.CurrentState = new DriveState(ParentMode, UserData);
            SetState(typeof(DriveState));
        }
    }
}