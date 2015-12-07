using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Allows for the message on the TombstoneItem to be edited or added, either way this form will get the job done. Will
    ///     limit the input of the epitaph also and do basic whitespace checks and trimming.
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class EpitaphEditor : Form<TravelInfo>
    {
        private StringBuilder _epitaphPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EpitaphEditor(IWindow gameMode) : base(gameMode)
        {
            _epitaphPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            //UserData.TombstoneItem.Epitaph =
            SetForm(typeof(EpitaphConfirm));
        }
    }
}