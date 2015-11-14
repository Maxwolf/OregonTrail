using System;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Spawns a new game mode in the game simulation while maintaining the state of previous one so when we bounce back we
    ///     can move from here to next state.
    /// </summary>
    public sealed class BuyInitialItemsState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one.
        /// </summary>
        public BuyInitialItemsState(IModeProduct gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Create text we will display to user about the store before they actually load that game mode.
            var _storeHelp = new StringBuilder();
            _storeHelp.Append($"{Environment.NewLine}Before leaving Independence you{Environment.NewLine}");
            _storeHelp.Append($"should buy equipment and{Environment.NewLine}");
            _storeHelp.Append(
                $"supplies. You have {UserData.StartingMonies.ToString("C2")} in{Environment.NewLine}{Environment.NewLine}");
            _storeHelp.Append($"cash, but you don't have to{Environment.NewLine}");
            _storeHelp.Append($"spend it all now.{Environment.NewLine}{Environment.NewLine}");
            return _storeHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = new IntroduceStoreState(parentGameMode, UserData);
            SetState(typeof (IntroduceStoreState));
        }
    }
}