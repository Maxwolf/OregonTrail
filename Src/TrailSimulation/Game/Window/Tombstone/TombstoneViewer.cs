using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Special window we can use to view existing tombstones as a dialog offering no changes or input from other players
    ///     letting them only look at the name and epitaph message left if any.
    /// </summary>
    [ParentWindow(GameWindow.Tombstone)]
    public sealed class TombstoneViewer : Window<TombstoneCommands, TombstoneInfo>
    {
        /// <summary>
        ///     References all of the text data that will be shown to user.
        /// </summary>
        private StringBuilder _tombstone;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Window" /> class.
        /// </summary>
        public TombstoneViewer()
        {
            _tombstone = new StringBuilder();
        }

        /// <summary>
        ///     Defines the current game Windows the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameWindow WindowCategory
        {
            get { return GameWindow.Tombstone; }
        }

        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game Windows might
        ///     need to at the top of menu selections.
        /// </summary>
        protected override string MenuHeader
        {
            get
            {
                // Check if the tombstone manager returned anything, if not then check for user data it's player death then.
                _tombstone.Clear();

                // Grab the current Tombstone based on players progress on the trail so far.
                Tombstone foundTombstone;
                GameSimulationApp.Instance.Graveyard.FindTombstone(
                    GameSimulationApp.Instance.Vehicle.Odometer,
                    out foundTombstone);

                // Add Tombstone message we want to show the player from Tombstone manager.
                _tombstone.AppendLine($"{Environment.NewLine}{foundTombstone}");

                // Adds the underlying reason for the games failure if it was not obvious to the player by now.
                _tombstone.AppendLine("All the members of");
                _tombstone.AppendLine("your party have");
                _tombstone.AppendLine($"died.{Environment.NewLine}");

                return _tombstone.ToString();
            }
        }

        /// <summary>
        ///     Forces the game simulation to reset itself to starting configuration. Allows the player to start a new game.
        /// </summary>
        private void RestartGame()
        {
            // Determine if we are showing the player a tombstone because they died.
            if (GameSimulationApp.Instance.Vehicle.PassengerLivingCount <= 0)
            {
                // Completely resets the game to default state it was in when it first started.
                GameSimulationApp.Instance.Restart();
                return;
            }

            // Return to travel mode menu if we are just looking at some other dead guy grave.
            ClearForm();
        }
    }
}