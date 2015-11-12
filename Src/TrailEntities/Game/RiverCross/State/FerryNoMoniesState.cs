using System.Text;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Attached when the user attempts to cross the river using the ferry, confirms they would like to but does not have
    ///     enough money at this point this state will be attached and explain to the user they cannot use the ferry and must
    ///     pick one of the other two options.
    /// </summary>
    public sealed class FerryNoMoniesState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FerryNoMoniesState(IModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _prompt = new StringBuilder();
            _prompt.AppendLine("You do not have enough");
            _prompt.AppendLine("monies to take the");
            _prompt.AppendLine("ferry.");
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //ParentMode.CurrentState = null;
            ClearState();
        }
    }
}