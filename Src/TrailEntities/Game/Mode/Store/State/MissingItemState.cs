using System;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Informs the player they need to purchase at least a single one of the specified SimItem in order to
    ///     continue. This is used in the new game mode to force the player to have at least one oxen to pull their vehicle in
    ///     order to start the simulation.
    /// </summary>
    public sealed class MissingItemState : DialogState<StoreInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public MissingItemState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var missingItem = new StringBuilder();
            missingItem.Append(
                $"You need to purchase at least a " +
                $"single {UserData.MissingItemEntity.DelineatingUnit} in order " +
                $"to begin your trip!{Environment.NewLine}");
            return missingItem.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            UserData.MissingItemEntity = null;
            //parentGameMode.CurrentState = null;
            ClearState();
        }
    }
}