using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Information about what fording a river means and how it works for the player vehicle and their party members.
    /// </summary>
    public sealed class FordRiverHelpState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FordRiverHelpState(IModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var fordRiver = new StringBuilder();
            fordRiver.AppendLine("To ford a river means to");
            fordRiver.AppendLine("pull your wagon across a");
            fordRiver.AppendLine("shallow part of the river,");
            fordRiver.AppendLine("with the oxen still");
            fordRiver.AppendLine("attached.");
            return fordRiver.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = new CaulkRiverHelpState(parentGameMode, UserData);
            SetState(typeof (CaulkRiverHelpState));
        }
    }
}