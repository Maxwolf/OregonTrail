// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/07/2015@5:01 AM

namespace TrailSimulation.Game
{
    using System;
    using System.Text;
    using Core;

    /// <summary>
    ///     Spawns a new game Windows in the game simulation while maintaining the state of previous one so when we bounce back
    ///     we
    ///     can move from here to next state.
    /// </summary>
    [ParentWindow(GameWindow.MainMenu)]
    public sealed class InitialItemsHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InitialItemsHelp" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public InitialItemsHelp(IWindow window) : base(window)
        {
            // Pass the game data to the simulation for each new game Windows state.
            GameSimulationApp.Instance.SetStartInfo(UserData);
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Create text we will display to user about the store before they actually load that game Windows.
            var _storeHelp = new StringBuilder();
            _storeHelp.AppendLine($"{Environment.NewLine}Before leaving Independence you");
            _storeHelp.AppendLine("should buy equipment and");
            _storeHelp.AppendLine($"supplies. You have {UserData.StartingMonies.ToString("C2")} in");
            _storeHelp.AppendLine("cash, but you don't have to");
            _storeHelp.AppendLine($"spend it all now.{Environment.NewLine}");
            return _storeHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (StoreHelp));
        }
    }
}