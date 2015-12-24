// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/21/2015@11:31 PM

namespace TrailSimulation.Game
{
    using Core;

    /// <summary>
    ///     Represents all of the needed forms and dialogs to end the game and restart it without any problems. This window
    ///     will be able to deal with both the win and fail state and route to the correct system depending on ending.
    /// </summary>
    public sealed class GameOver : Window<GameOverCommands, GameOverInfo>
    {
        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow WindowCategory
        {
            get { return GameWindow.GameOver; }
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
                SetForm(typeof (GameWin));
                return;
            }

            // Check if player reached end of the trail.
            if (GameSimulationApp.Instance.Vehicle.PassengersDead)
            {
                SetForm(typeof (GameFail));
                return;
            }

            // Nothing took above we are going to detach this window.
            RemoveWindowNextTick();
        }
    }
}