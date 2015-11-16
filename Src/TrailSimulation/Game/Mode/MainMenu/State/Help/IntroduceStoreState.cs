using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Introduces the player to the concept of a store as being run by a person by the name of Matt.
    /// </summary>
    [RequiredMode(GameMode.MainMenu)]
    public sealed class IntroduceStoreState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public IntroduceStoreState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.Prompt; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _storeHelp = new StringBuilder();
            _storeHelp.Append($"{Environment.NewLine}You can buy whatever you need at{Environment.NewLine}");
            _storeHelp.Append($"Matt's General Store.{Environment.NewLine}{Environment.NewLine}");
            return _storeHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ParentMode.RemoveModeNextTick();
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.Store);
        }
    }
}