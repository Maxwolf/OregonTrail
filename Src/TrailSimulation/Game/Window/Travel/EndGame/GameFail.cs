using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used when the party leader dies, no matter what happens this prevents the rest of the game from moving forward and
    ///     everybody dies. This state offers up the chance for the person to leave a personal epitaph of their existence as a
    ///     warning or really whatever. The fun is not knowing what they will say!
    /// </summary>
    [ParentWindow(Windows.Travel)]
    public sealed class GameFail : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public GameFail(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Create a new TombstoneItem that will become the players grave.
            UserData.TombstoneItem = new TombstoneItem();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var tombstone = new StringBuilder();

            // Add TombstoneItem message with epitaph if the user chose to input one for us to save.
            tombstone.AppendLine(UserData.TombstoneItem.ToString());
            tombstone.AppendLine("All of the people");
            tombstone.Append("in your party have died.");

            return tombstone.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Ask the player if they would like to write a custom message on their grave...
            SetForm(typeof(EpitaphQuestion));
        }
    }
}