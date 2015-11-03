namespace TrailEntities
{
    /// <summary>
    ///     Glorified options menu for the game that allows player to remove top ten high scores, remove saved games, erase
    ///     tombstone messages, etc.
    /// </summary>
    public sealed class OptionsMode : GameMode<OptionCommands>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.OptionsMode" /> class.
        /// </summary>
        public OptionsMode() : base(false)
        {
            AddCommand(SeeCurrentTopTen, OptionCommands.SeeCurrentTopTen, "See the current Top Ten list");
            AddCommand(SeeOriginalTopTen, OptionCommands.SeeOriginalTopTen, "See the original Top Ten list");
            AddCommand(EraseCurrentTopTen, OptionCommands.EraseCurrentTopTen, "Erase the current Top Ten list");
            AddCommand(EraseTombstoneMessages, OptionCommands.EraseTomstoneMessages, "Erase the tombstone messages");
            AddCommand(EraseSavedGames, OptionCommands.EraseSavedGames, "Erase saved games");
            AddCommand(ReturnToMainMenu, OptionCommands.ReturnToMainMenu, "Return to the main menu");
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.ManagementOptions; }
        }

        /// <summary>
        ///     Removes the management options game mode and returns to main menu which should be below it.
        /// </summary>
        private void ReturnToMainMenu()
        {
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Delete any save games from the slots they are holding. Any saved progress will be lost.
        /// </summary>
        private void EraseSavedGames()
        {
        }

        /// <summary>
        ///     Removes any custom tombstone messages from the trail, there are two opportunities in the game where if party leader
        ///     dies they can leave a tombstone on the trail for future travelers to find.
        /// </summary>
        private void EraseTombstoneMessages()
        {
        }

        /// <summary>
        ///     Resets the current top ten list back to hard-coded defaults.
        /// </summary>
        private void EraseCurrentTopTen()
        {
        }

        /// <summary>
        ///     Shows the original top ten list as it is known internally as a hard-coded list.
        /// </summary>
        private void SeeOriginalTopTen()
        {
        }

        /// <summary>
        ///     Shows the current top ten list as it is known from flat-file JSON list, players high scores can go into this list.
        /// </summary>
        private void SeeCurrentTopTen()
        {
        }
    }
}