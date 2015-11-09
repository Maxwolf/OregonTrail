using System;
using System.Reflection;
using System.Text;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Glorified options menu for the game that allows player to remove top ten high scores, remove saved games, erase
    ///     tombstone messages, etc.
    /// </summary>
    public sealed class OptionsGameMode : ModeProduct
    {
        /// <summary>
        ///     References all of the possible options we want to keep track of while moving between management options gameMode.
        /// </summary>
        private OptionInfo _optionInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct.OptionsGameMode" /> class.
        /// </summary>
        public OptionsGameMode() : base(false)
        {
            // Info object for states.
            _optionInfo = new OptionInfo();

            // Header text.
            var headerText = new StringBuilder();
            headerText.Append($"{Environment.NewLine}The Oregon Trail{Environment.NewLine}");
            headerText.Append(
                $"Version: {Assembly.GetExecutingAssembly().GetName().Version}{Environment.NewLine}{Environment.NewLine}");
            headerText.Append($"Management Options{Environment.NewLine}{Environment.NewLine}");
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            // Commands.
            AddCommand(SeeOriginalTopTen, OptionCommands.SeeOriginalTopTen, "See the original Top Ten list");
            AddCommand(EraseCurrentTopTen, OptionCommands.EraseCurrentTopTen, "Erase the current Top Ten list");
            AddCommand(EraseTombstoneMessages, OptionCommands.EraseTomstoneMessages, "Erase the tombstone messages");
            AddCommand(ReturnToMainMenu, OptionCommands.ReturnToMainMenu, "Return to the main menu");
        }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.Options; }
        }

        /// <summary>
        ///     Removes the management options game gameMode and returns to main menu which should be below it.
        /// </summary>
        private void ReturnToMainMenu()
        {
            SetShouldRemoveMode();
        }

        /// <summary>
        ///     Removes any custom tombstone messages from the trail, there are two opportunities in the game where if party leader
        ///     dies they can leave a tombstone on the trail for future travelers to find.
        /// </summary>
        private void EraseTombstoneMessages()
        {
            AddState(typeof(EraseTombstoneState));
        }

        /// <summary>
        ///     Resets the current top ten list back to hard-coded defaults.
        /// </summary>
        private void EraseCurrentTopTen()
        {
            AddState(typeof(EraseCurrentTopTenState));
        }

        /// <summary>
        ///     Shows the original top ten list as it is known internally as a hard-coded list.
        /// </summary>
        private void SeeOriginalTopTen()
        {
            AddState(typeof(OriginalTopTenState));
        }
    }
}