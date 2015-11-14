using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    public sealed class CaulkRiverHelpState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CaulkRiverHelpState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _caulkWagon = new StringBuilder();
            _caulkWagon.AppendLine("To caulk the wagon means to");
            _caulkWagon.AppendLine("seal it so that no water can");
            _caulkWagon.AppendLine("get in. The wagon can then");
            _caulkWagon.AppendLine("be floated across like a");
            _caulkWagon.AppendLine("boat");
            return _caulkWagon.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = new FerryHelpState(parentGameMode, UserData);
            SetState(typeof (FerryHelpState));
        }
    }
}