// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaulkRiverHelp.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   The caulk river help.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     The caulk river help.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class CaulkRiverHelp : InputForm<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="CaulkRiverHelp"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public CaulkRiverHelp(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
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

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new FerryHelp(parentGameMode, UserData);
            SetForm(typeof (FerryHelp));
        }
    }
}