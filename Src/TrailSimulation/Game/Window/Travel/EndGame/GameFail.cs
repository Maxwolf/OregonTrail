using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Fired when the simulation has determined the player has died. It specifically only attaches at this time. The flow
    ///     for death like this is to first show the player the failure state like this, then ask if they want to leave an
    ///     epitaph, process that decision, confirm it, and finally show the viewer that will also show the reason why the
    ///     player died using description attribute from an enumeration value that determines how they died.
    /// </summary>
    [ParentWindow(SimulationModule.Travel)]
    public sealed class GameFail : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public GameFail(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Creates the tombstone that will represent the player leader (since he is most important).
            UserData.TombstoneItem = new TombstoneItem();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _tombstone = new StringBuilder();

            // Adds TombstoneItem from the user data because the player died.
            _tombstone.AppendLine($"{Environment.NewLine}{UserData.TombstoneItem}");

            return _tombstone.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (EpitaphQuestion));
        }
    }
}