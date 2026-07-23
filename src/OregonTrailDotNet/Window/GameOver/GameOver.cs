// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using WolfCurses;
using WolfCurses.Core;
using WolfCurses.Window;

namespace OregonTrailDotNet.Window.GameOver
{
    /// <summary>
    ///     Controls the process of ending the current game simulation depending on if the player won or lost. This window can
    ///     be attached at any point by any other window, or form in order to facilitate the game being able to trigger a game
    ///     over scenario no matter what is happening.
    /// </summary>
    public sealed class GameOver : Window<GameOverCommandsEnum, GameOverInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        // ReSharper disable once UnusedMember.Global
        public GameOver(SimulationApp simUnit) : base(simUnit)
        {
        }

        /// <summary>
        ///     Resets the input prompt so a context-specific prompt set by one form does not leak into the next.
        /// </summary>
        protected override void OnFormChange()
        {
            base.OnFormChange();
            PromptText = SceneGraph.PROMPT_TEXT_DEFAULT;

            // A scene form's tune must not outlive it: stop the victory music when a plain form (the scoring
            // table) takes over — the same guard Travel and the Graveyard carry.
            if (GameSimulationApp.PresentationEnabled &&
                CurrentForm is not OregonTrailDotNet.Presentation.SceneForm<GameOverInfo>)
                OregonTrailDotNet.Presentation.Audio.Music.Stop();
        }

        /// <summary>
        ///     Called after the window has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            // Reaching the last location wins; with presentation on the victory is the Willamette card + tune.
            if (GameSimulationApp.Instance.Trail.CurrentLocation.LastLocation)
            {
                SetForm(GameSimulationApp.PresentationEnabled ? typeof(VictoryScene) : typeof(GameWin));
                return;
            }

            // A dead party fails; with presentation on the death screen is the wrecked wagon.
            if (GameSimulationApp.Instance.Vehicle.PassengersDead)
            {
                SetForm(GameSimulationApp.PresentationEnabled ? typeof(DeathScene) : typeof(GameFail));
                return;
            }

            // Nothing took above we are going to detach this window.
            RemoveWindowNextTick();
        }
    }
}