// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trading.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Handles the interaction of the player party and another AI controlled party that offers up items for trading which
//   the player can choose to accept or not.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Handles the interaction of the player party and another AI controlled party that offers up items for trading which
    ///     the player can choose to accept or not.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class Trading : Form<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="Trading"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public Trading(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}