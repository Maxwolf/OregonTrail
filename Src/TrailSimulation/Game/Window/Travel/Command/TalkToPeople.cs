// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TalkToPeople.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Attaches a game state that will loop through random advice that is associated with the given point of interest.
//   This is not a huge list and players will eventually see the same advice if they keep coming back, only one piece of
//   advice should be shown and one day will advance in the simulation to prevent the player from just spamming it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using System;
    using Core;

    /// <summary>
    ///     Attaches a game state that will loop through random advice that is associated with the given point of interest.
    ///     This is not a huge list and players will eventually see the same advice if they keep coming back, only one piece of
    ///     advice should be shown and one day will advance in the simulation to prevent the player from just spamming it.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class TalkToPeople : Form<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="TalkToPeople"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public TalkToPeople(IWindow window) : base(window)
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