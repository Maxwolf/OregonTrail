// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.Travel.TalkToPeople
{
    using System;
    using System.Collections.Generic;
    using Entity.Location.Point;

    /// <summary>
    ///     Attaches a game state that will loop through random advice that is associated with the given point of interest.
    ///     This is not a huge list and players will eventually see the same advice if they keep coming back, only one piece of
    ///     advice should be shown and one day will advance in the simulation to prevent the player from just spamming it.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class TalkToPeople : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TalkToPeople" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public TalkToPeople(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The dialog prompt text.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Grab instance of game simulation.
            var game = GameSimulationApp.Instance;

            // Figure out what type of advice we want.
            List<Advice> advice;
            if (game.Trail.IsFirstLocation)
            {
                // First location gives tutorial like advice about possible pitfalls.
                advice = new List<Advice>(AdviceRegistry.Tutorial);
            }
            else if (game.Trail.CurrentLocation is ForkInRoad)
            {
                // Fork in road is always considered mountainous area.
                advice = new List<Advice>(AdviceRegistry.Mountain);
            }
            else if (game.Trail.CurrentLocation is Landmark)
            {
                // Landmarks have people that miss civilization and been on road for a while.
                advice = new List<Advice>(AdviceRegistry.Landmark);
            }
            else if (game.Trail.CurrentLocation is Settlement)
            {
                // Settlements have shops, more people, and lots of people scared about stories they hear.
                advice = new List<Advice>(AdviceRegistry.Settlement);
            }
            else if (game.Trail.CurrentLocation is RiverCrossing)
            {
                // Can talk to people working near the river as operators or as people about to cross.
                advice = new List<Advice>(AdviceRegistry.River);
            }
            else
            {
                // Not sure what type of location this is so use ending quotes.
                advice = new List<Advice>(AdviceRegistry.Ending);
            }

            // Grab single random piece of advice from that collection.
            var randomAdvice = advice.PickRandom(1).FirstOrDefault();

            // Render out the advice to the form.
            return randomAdvice == null
                ? $"{Environment.NewLine}AdviceRegistry.DEFAULTADVICE{Environment.NewLine}"
                : $"{Environment.NewLine}{randomAdvice.Name},{Environment.NewLine}{randomAdvice.Quote.WordWrap()}{Environment.NewLine}";
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // Return to travel menu.
            ClearForm();
        }
    }
}