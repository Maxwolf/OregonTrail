// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Window.Travel.RiverCrossing.Ferry;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing.Help
{
    /// <summary>
    ///     The caulk river help.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class CaulkRiverHelp : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CaulkRiverHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
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
            var caulkWagon = new StringBuilder();
            caulkWagon.AppendLine($"{Environment.NewLine}To caulk the wagon means to");
            caulkWagon.AppendLine("seal it so that no water can");
            caulkWagon.AppendLine("get in. The wagon can then");
            caulkWagon.AppendLine("be floated across like a");
            caulkWagon.AppendLine($"boat{Environment.NewLine}");
            return caulkWagon.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new FerryHelp(parentGameMode, UserData);
            SetForm(typeof(FerryHelp));
        }
    }
}