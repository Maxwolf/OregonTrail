// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hunting.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
//   kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
//   can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class Hunting : Form<TravelInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hunting"/> class. 
        /// This constructor will be used by the other one
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        public Hunting(IWindow window) : base(window)
        {
        }

        /// <summary>
        /// The use bullets.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void UseBullets(int amount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The add food.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void AddFood(int amount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update vehicle.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">
        /// Contents of the input buffer which didn't match any known command in parent game mode.
        /// </param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}