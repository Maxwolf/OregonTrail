using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    [ParentWindow(Windows.Travel)]
    public sealed class CaulkRiverHelp : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CaulkRiverHelp(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _caulkWagon = new StringBuilder();
            _caulkWagon.AppendLine($"{Environment.NewLine}To caulk the wagon means to");
            _caulkWagon.AppendLine("seal it so that no water can");
            _caulkWagon.AppendLine("get in. The wagon can then");
            _caulkWagon.AppendLine("be floated across like a");
            _caulkWagon.AppendLine($"boat{Environment.NewLine}");
            return _caulkWagon.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.State = new FerryHelp(parentGameMode, UserData);
            SetForm(typeof (FerryHelp));
        }
    }
}