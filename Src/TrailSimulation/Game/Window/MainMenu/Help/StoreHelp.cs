using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Introduces the player to the concept of a store as being run by a person by the name of Matt.
    /// </summary>
    [ParentWindow(Windows.MainMenu)]
    public sealed class StoreHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreHelp(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
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
            // Closes main menu and drops back to travel Windows at the bottom level which should have store already open and ready.
            ParentMode.RemoveModeNextTick();
        }
    }
}