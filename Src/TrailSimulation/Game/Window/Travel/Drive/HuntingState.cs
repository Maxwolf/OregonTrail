using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [RequiredWindow(Windows.Travel)]
    public sealed class HuntingState : Form<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public HuntingState(IWindow gameMode) : base(gameMode)
        {
        }

        public void UseBullets(int amount)
        {
            throw new NotImplementedException();
        }

        public void AddFood(int amount)
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}