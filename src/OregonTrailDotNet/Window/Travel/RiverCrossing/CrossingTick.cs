// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.River;
using OregonTrailDotNet.Event.Vehicle;
using WolfCurses.Core;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Control;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.RiverCrossing
{
    /// <summary>
    ///     Runs the player over the river based on the crossing information. Depending on what happens a message will be
    ///     printed to the screen explaining what happened before defaulting back to travel game Windows.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class CrossingTick : Form<TravelInfo>
    {
        /// <summary>
        ///     String builder that will hold all the data about our river crossing as it occurs.
        /// </summary>
        private readonly StringBuilder _crossingPrompt;

        /// <summary>
        ///     The crossing itself — payment, progress, and dangers — shared verbatim with the graphical scene.
        /// </summary>
        private CrossingSimulation _sim;

        /// <summary>
        ///     Animated sway bar that prints out as text, ping-pongs back and fourth between left and right side, moved by
        ///     stepping it with tick.
        /// </summary>
        private readonly MarqueeBar _marqueeBar;

        /// <summary>
        ///     Holds the text related to animated sway bar, each tick of simulation steps it.
        /// </summary>
        private string _swayBarText;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossingTick" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public CrossingTick(IWindow window) : base(window)
        {
            // Create the string builder for holding all our text about river crossing as it happens.
            _crossingPrompt = new StringBuilder();

            // Animated sway bar.
            _marqueeBar = new MarqueeBar();
            _swayBarText = _marqueeBar.Step();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer => false;

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput => _sim is { Finished: true };

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // The crossing itself — parking the vehicle and taking the agreed payment — is the shared simulation's.
            _sim = new CrossingSimulation(UserData.River);
            _sim.Begin();
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            // Clears the string buffer for this render pass.
            _crossingPrompt.Clear();

            // Ping-pong progress bar to show that we are moving.
            _crossingPrompt.AppendLine($"{Environment.NewLine}{_swayBarText}");

            // Get instance of game simulation.
            var game = GameSimulationApp.Instance;

            // Framed panel showing basic status of the vehicle and total river crossing progress; the river's name titles it.
            var body = new StringBuilder();
            body.AppendLine($"{game.Time.Date}");
            body.AppendLine($"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");
            body.AppendLine($"Health: {game.Vehicle.PassengerHealthStatus.ToDescriptionAttribute()}");
            body.AppendLine($"Crossing By: {UserData.River.CrossingType}");
            body.AppendLine($"River width: {UserData.River.RiverWidth:N0} feet");
            body.Append($"River crossed: {_sim?.FeetCrossed ?? 0:N0} feet");
            _crossingPrompt.AppendLine(FramedPanel.Render(game.Trail.CurrentLocation.Name, body.ToString()));

            // Wait for user input...
            if (_sim is { Finished: true })
                _crossingPrompt.AppendLine(InputManager.PRESSENTER);

            return _crossingPrompt.ToString();
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // Skip system ticks.
            if (systemTick)
                return;

            // Stop crossing if we have finished.
            if (_sim is not { Finished: false })
                return;

            // Advance the progress bar, step it to next phase.
            _swayBarText = _marqueeBar.Step();

            // The crossing itself — progress, the finishing freeze, the skip-day turn, and the danger rolls — is
            // the shared simulation's, in the characterized order.
            _sim.Tick();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if we are still crossing the river.
            if (_sim is not { Finished: true })
                return;

            SetForm(typeof(CrossingResult));
        }
    }
}