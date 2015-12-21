// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseIndianConfirm.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Confirms with the player the decision they made about crossing the riving using the Indian guide in exchange for a
//   set amount of clothing. This form we will actually process that transaction and then let the Indian take the player
//   across the river like he promised.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Confirms with the player the decision they made about crossing the riving using the Indian guide in exchange for a
    ///     set amount of clothing. This form we will actually process that transaction and then let the Indian take the player
    ///     across the river like he promised.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class UseIndianConfirm : InputForm<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="UseIndianConfirm"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public UseIndianConfirm(IWindow window) : base(window)
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
            var _prompt = new StringBuilder();
            _prompt.AppendLine($"{Environment.NewLine}The Shoshoni guide will help");
            _prompt.AppendLine("you float your wagon across.");
            return _prompt.ToString();
        }

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Player has enough clothing to satisfy the Indians cost.
            SetForm(typeof (CrossingTick));
        }
    }
}