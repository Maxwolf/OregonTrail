using System;
using System.Reflection;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Glorified options menu for the game that allows player to remove top ten high scores, remove saved games, erase
    ///     tombstone messages, etc.
    /// </summary>
    public sealed class OptionsMode : ModeProduct<OptionCommands, OptionInfo>
    {
        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override Mode Mode
        {
            get { return Mode.Options; }
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
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
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        public override void OnModeActivate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation adds a game mode that is not this mode. Used to execute code in other modes that are not
        ///     the active mode anymore one last time.
        /// </summary>
        public override void OnModeAdded()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Removes the management options game mode and returns to main menu which should be below it.
        /// </summary>
        private void ReturnToMainMenu()
        {
            //CurrentState = null;
            ClearState();
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Removes any custom tombstone messages from the trail, there are two opportunities in the game where if party leader
        ///     dies they can leave a tombstone on the trail for future travelers to find.
        /// </summary>
        private void EraseTombstoneMessages()
        {
            //CurrentState = new EraseTombstoneState(this, _optionInfo);
            SetState(typeof (EraseTombstoneState));
        }

        /// <summary>
        ///     Resets the current top ten list back to hard-coded defaults.
        /// </summary>
        private void EraseCurrentTopTen()
        {
            //CurrentState = new EraseCurrentTopTenState(this, _optionInfo);
            SetState(typeof (EraseCurrentTopTenState));
        }

        /// <summary>
        ///     Shows the original top ten list as it is known internally as a hard-coded list.
        /// </summary>
        private void SeeOriginalTopTen()
        {
            //CurrentState = new OriginalTopTenState(this, _optionInfo);
            SetState(typeof (OriginalTopTenState));
        }
    }
}