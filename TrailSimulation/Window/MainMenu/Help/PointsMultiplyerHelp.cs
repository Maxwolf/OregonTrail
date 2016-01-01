// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Window.MainMenu.Help
{
    using System;
    using System.Text;
    using WolfCurses.Window;
    using WolfCurses.Window.Form;
    using WolfCurses.Window.Form.Input;

    /// <summary>
    ///     Third and final panel on point information, explains how players profession selection affects final scoring as a
    ///     multiplier since starting as a banker is a handicap.
    /// </summary>
    [ParentWindow(typeof (MainMenu))]
    public sealed class PointsMultiplyerHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PointsMultiplyerHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public PointsMultiplyerHelp(IWindow window) : base(window)
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
            var _pointsProfession = new StringBuilder();
            _pointsProfession.Append(
                $"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}{Environment.NewLine}");
            _pointsProfession.AppendLine("You receive points for your");
            _pointsProfession.AppendLine("occupation in the new land.");
            _pointsProfession.AppendLine("Because more farmers and");
            _pointsProfession.AppendLine("carpenters were needed than");
            _pointsProfession.AppendLine("bankers, you receive double");
            _pointsProfession.AppendLine("points upon arriving in Oregon");
            _pointsProfession.AppendLine("as a carpenter, and triple");
            _pointsProfession.AppendLine($"points for arriving as a farmer.{Environment.NewLine}");
            return _pointsProfession.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearForm();
        }
    }
}