// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RiverCrossHelp.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Shown to the user the first time they cross a river, this way it can be explained to them they must cross it in
//   order to continue and there is no going around. We tell them how deep the water is and how many feed across the
//   river is they will need to travel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shown to the user the first time they cross a river, this way it can be explained to them they must cross it in
    ///     order to continue and there is no going around. We tell them how deep the water is and how many feed across the
    ///     river is they will need to travel.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class RiverCrossHelp : InputForm<TravelInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiverCrossHelp"/> class. 
        /// This constructor will be used by the other one
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        public RiverCrossHelp(IWindow window) : base(window)
        {
        }

        /// <summary>
        /// Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Generates a new river with randomized width and depth.
            UserData.GenerateRiver();

            var riverPrompt = new StringBuilder();
            riverPrompt.AppendLine($"{Environment.NewLine}You must cross the river in");
            riverPrompt.AppendLine("order to continue. The");
            riverPrompt.AppendLine("river at this point is");
            riverPrompt.AppendLine($"currently {UserData.River.RiverWidth} feet across,");
            riverPrompt.AppendLine($"and {UserData.River.RiverDepth} feet deep in the");
            riverPrompt.AppendLine($"middle.{Environment.NewLine}");
            return riverPrompt.ToString();
        }

        /// <summary>
        /// Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">
        /// The response the dialog parsed from simulation input buffer.
        /// </param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (RiverCross));
        }
    }
}