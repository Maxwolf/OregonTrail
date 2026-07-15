// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using OregonTrailDotNet.Module.Time;
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
        }

        /// <summary>
        ///     Called after the window has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            // Check if passengers in the vehicle are dead.
            if (GameSimulationApp.Instance.Trail.CurrentLocation.LastLocation)
            {
                SetForm(typeof(GameWin));
                return;
            }

            // Check if player reached end of the trail.
            if (GameSimulationApp.Instance.Vehicle.PassengersDead)
            {
                SetForm(typeof(GameFail));
                return;
            }

            // The journey ran out of time (20+ weeks) with the party still alive but short of Oregon; tabulate the score for
            // however far they made it.
            if (GameSimulationApp.Instance.Time.TotalDays >= TimeModule.MaxTravelDays)
            {
                SetForm(typeof(FinalPoints));
                return;
            }

            // Nothing took above we are going to detach this window.
            RemoveWindowNextTick();
        }
    }
}