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
        ///     Determines if this state has performed it's duties and helped get the players and their vehicle across the river.
        /// </summary>
        private bool _finishedCrossingRiver;

        /// <summary>
        ///     Animated sway bar that prints out as text, ping-pongs back and fourth between left and right side, moved by
        ///     stepping it with tick.
        /// </summary>
        private readonly MarqueeBar _marqueeBar;

        /// <summary>
        ///     Defines the current amount of feet we have crossed of the river, this will tick up to the total length of the
        ///     river.
        /// </summary>
        private int _riverCrossingOfTotalWidth;

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

            // Sets the crossing percentage to zero.
            _riverCrossingOfTotalWidth = 0;
            _finishedCrossingRiver = false;
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
        public override bool AllowInput => _finishedCrossingRiver;

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Grab instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Park the vehicle if it is not somehow by now.
            game.Vehicle.Status = VehicleStatusEnum.Stopped;

            // Check if ferry operator wants players monies for trip across river.
            if ((UserData.River.FerryCost > 0) &&
                (game.Vehicle.Inventory[EntitiesEnum.Cash].TotalValue > UserData.River.FerryCost))
            {
                game.Vehicle.Inventory[EntitiesEnum.Cash].ReduceQuantity((int) UserData.River.FerryCost);

                // Clear out the cost for the ferry since it has been paid for now.
                UserData.River.FerryCost = 0;
            }

            // Check if the Indian guide wants his clothes for the trip that you agreed to. Use >= to match the offer
            // gate in IndianGuidePrompt (Clothes >= IndianCost); a strict > let a party with exactly the cost cross free.
            if ((UserData.River.IndianCost > 0) &&
                (game.Vehicle.Inventory[EntitiesEnum.Clothes].Quantity >= UserData.River.IndianCost))
            {
                game.Vehicle.Inventory[EntitiesEnum.Clothes].ReduceQuantity(UserData.River.IndianCost);

                // Clear out the cost for the ferry since it has been paid for now.
                UserData.River.IndianCost = 0;
            }
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
            body.Append($"River crossed: {_riverCrossingOfTotalWidth:N0} feet");
            _crossingPrompt.AppendLine(FramedPanel.Render(game.Trail.CurrentLocation.Name, body.ToString()));

            // Wait for user input...
            if (_finishedCrossingRiver)
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
            if (_finishedCrossingRiver)
                return;

            // Grab instance of game simulation for easy reading.
            var game = GameSimulationApp.Instance;

            // Advance the progress bar, step it to next phase.
            _swayBarText = _marqueeBar.Step();

            // Increment the amount we have floated over the river.
            _riverCrossingOfTotalWidth += game.Random.Next(1, UserData.River.RiverWidth/4);

            // Check to see if we will finish crossing river before crossing more.
            if (_riverCrossingOfTotalWidth >= UserData.River.RiverWidth)
            {
                _riverCrossingOfTotalWidth = UserData.River.RiverWidth;
                _finishedCrossingRiver = true;
                return;
            }

            // River crossing will allow ticking of people, vehicle, and other important events but others like consuming food are disabled.
            GameSimulationApp.Instance.TakeTurn(true);

            // Attempt to throw a random event related to some failure happening with river crossing.
            switch (UserData.River.CrossingType)
            {
                case RiverCrossChoiceEnum.Ford:
                    // Whatever the river is going to do, it does it midstream, and only once.
                    if (!UserData.River.DisasterHappened &&
                        (_riverCrossingOfTotalWidth >= UserData.River.RiverWidth/2))
                        FordMidstream(game);
                    break;
                case RiverCrossChoiceEnum.Float:
                    if (!UserData.River.DisasterHappened &&
                        (_riverCrossingOfTotalWidth >= UserData.River.RiverWidth/2))
                        FloatMidstream(game);
                    break;
                case RiverCrossChoiceEnum.Ferry:
                case RiverCrossChoiceEnum.Indian:
                    game.EventDirector.TriggerEventByType(game.Vehicle, EventCategoryEnum.RiverCross);
                    break;
                case RiverCrossChoiceEnum.None:
                case RiverCrossChoiceEnum.WaitForWeather:
                case RiverCrossChoiceEnum.GetMoreInformation:
                    throw new InvalidOperationException(
                        $"Invalid river crossing result choice {UserData.River.CrossingType}.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Works out what a ford costs the party, which is decided almost entirely by how deep the water is. Anything up
        ///     to two and a half feet is safe and the only question is what the riverbed does to you; up to three feet soaks
        ///     the supplies and costs a day; deeper than that and the river starts taking things, and the deeper it runs the
        ///     more it takes - it is never certain, but by five or six feet it very nearly is.
        /// </summary>
        /// <param name="game">Running simulation.</param>
        private void FordMidstream(GameSimulationApp game)
        {
            var river = UserData.River;
            river.DisasterHappened = true;

            // Shallow enough to walk across; only the going underfoot is in question.
            if (river.RiverDepth < RiverGenerator.SafeFordDepth)
            {
                switch (river.RiverBottom)
                {
                    case RiverBottomEnum.Muddy when game.Random.NextDouble() < 0.4:
                        game.EventDirector.TriggerEvent(game.Vehicle, typeof(StuckInMud));
                        break;
                    case RiverBottomEnum.Rough when game.Random.NextDouble() < 0.16:
                        game.EventDirector.TriggerEvent(game.Vehicle, typeof(VehicleFloods));
                        break;
                }

                return;
            }

            // Deep enough to come over the wagon bed and wet everything in it, but not to carry it off.
            if (river.RiverDepth < RiverGenerator.DangerousFordDepth)
            {
                game.EventDirector.TriggerEvent(game.Vehicle, typeof(SuppliesWet));
                return;
            }

            // Past that the river is genuinely taking things, and how much rides on how deep it is.
            if (game.Random.NextDouble() < river.RiverDepth/10.0)
                game.EventDirector.TriggerEvent(game.Vehicle, typeof(VehicleWashOut));
        }

        /// <summary>
        ///     Works out what floating the wagon costs. Depth is not the danger here - the wagon is already swimming - it is
        ///     how fast the water is moving, so a deep slow river is a better bet than a shallow fast one.
        /// </summary>
        /// <param name="game">Running simulation.</param>
        private void FloatMidstream(GameSimulationApp game)
        {
            var river = UserData.River;
            river.DisasterHappened = true;

            // Still water carries a caulked wagon across without complaint.
            if (river.RiverDepth <= RiverGenerator.SafeFordDepth)
                return;

            if (game.Random.NextDouble() < river.RiverSpeed/20.0)
                game.EventDirector.TriggerEvent(game.Vehicle, typeof(VehicleFloods));
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if we are still crossing the river.
            if (_riverCrossingOfTotalWidth < UserData.River.RiverWidth)
                return;

            SetForm(typeof(CrossingResult));
        }
    }
}