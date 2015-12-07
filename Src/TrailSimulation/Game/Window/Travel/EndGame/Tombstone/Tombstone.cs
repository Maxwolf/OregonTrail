using System;
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
    public sealed class Tombstone : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public Tombstone(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var tombstone = new StringBuilder();

            // TODO: Add tombstone message with epitaph if the user chose to input one for us to save.

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
            throw new NotImplementedException();
        }
    }
}