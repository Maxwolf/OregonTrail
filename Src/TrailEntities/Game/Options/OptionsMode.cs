using System;
using System.Reflection;
using System.Text;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Glorified options menu for the game that allows player to remove top ten high scores, remove saved games, erase
    ///     tombstone messages, etc.
    /// </summary>
    [GameMode(ModeType.Options)]
    // ReSharper disable once UnusedMember.Global
    public sealed class OptionsMode : ModeProduct<OptionCommands>
    {
        /// <summary>
        ///     References all of the possible options we want to keep track of while moving between management options mode.
        /// </summary>
        private OptionInfo _optionInfo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.OptionsMode" /> class.
        /// </summary>
        public OptionsMode() : base(false)
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
            AddCommand(SeeOriginalTopTen, OptionCommands.SeeOriginalTopTen);
            AddCommand(EraseCurrentTopTen, OptionCommands.EraseCurrentTopTen);
            AddCommand(EraseTombstoneMessages, OptionCommands.EraseTomstoneMessages);
            AddCommand(ReturnToMainMenu, OptionCommands.ReturnToMainMenu);
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Options; }
        }

        /// <summary>
        ///     Removes the management options game mode and returns to main menu which should be below it.
        /// </summary>
        private void ReturnToMainMenu()
        {
            CurrentState = null;
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Removes any custom tombstone messages from the trail, there are two opportunities in the game where if party leader
        ///     dies they can leave a tombstone on the trail for future travelers to find.
        /// </summary>
        private void EraseTombstoneMessages()
        {
            CurrentState = new EraseTombstoneState(this, _optionInfo);
        }

        /// <summary>
        ///     Resets the current top ten list back to hard-coded defaults.
        /// </summary>
        private void EraseCurrentTopTen()
        {
            CurrentState = new EraseCurrentTopTenState(this, _optionInfo);
        }

        /// <summary>
        ///     Shows the original top ten list as it is known internally as a hard-coded list.
        /// </summary>
        private void SeeOriginalTopTen()
        {
            CurrentState = new OriginalTopTenState(this, _optionInfo);
        }
    }
}