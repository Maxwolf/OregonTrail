using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached if the player decides to rest by the river for a specified amount of time. Events will still tick here as
    ///     the number of days is moved through.
    /// </summary>
    [RequiredMode(GameMode.RiverCrossing)]
    public sealed class CampByRiverState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CampByRiverState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            return "You camp near the river for a day." + Environment.NewLine;
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = null;
            ClearState();
        }
    }
}