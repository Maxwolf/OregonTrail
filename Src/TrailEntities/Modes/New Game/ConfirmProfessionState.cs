using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Asks the user to confirm their selection for a given profession, if they select anything other than YES we will
    ///     restart the select profession state again.
    /// </summary>
    public sealed class ConfirmProfessionState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmProfessionState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {


            return $"Selected profession {UserData.PlayerProfession} for party leader " +
                   $"{UserData.PlayerNames[0]}.\n Is this correct? Y/N";
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "Y":
                    // User is happy with their profession, we will now add a store game mode so they can buy things with starting monies.
                    Mode.CurrentState = new BuyInitialItemsState(Mode, UserData);
                    break;
                default:
                    // User is not happy with profession choice and is going to reset the profession selector and try again.
                    Mode.CurrentState = new SelectProfessionState(Mode, UserData);
                    break;
            }
        }
    }
}